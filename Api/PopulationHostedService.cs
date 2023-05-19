using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using BL.Services;
using BL.ViewModels;
using MongoDB.Driver;

namespace Api
{
    public class PopulationHostedService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly IHubContext<SuscriptionHub> _suscriptionHub;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IEventoService? _eventoService;
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<NotificationViewModel> _notificationCollection;
        public PopulationHostedService(IHubContext<SuscriptionHub> _suscriptionHub,
            IServiceScopeFactory _serviceScopeFactory, IMongoClient mongoClient)
        {
            this._suscriptionHub = _suscriptionHub;
            this._serviceScopeFactory = _serviceScopeFactory;
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("suscriptiondb");
            _notificationCollection = database.GetCollection<NotificationViewModel>("notification");
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        /// <summary>
        /// Método de inicio
        /// </summary>
        /// <remarks>
        /// Se crea un nuevo objeto Timer que ejecutará el método SendInfo de forma 
        /// repetida cada 5 segundos en segundo plano al iniciar la aplicación
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendInfo, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Método para la gestión de notificaciones
        /// </summary>
        /// <remarks>
        /// Se inicia desde una consulta principal donde se obtiene los eventos que coincidan con la fecha actual y 
        /// un estádo válido. Los eventos que están por iniciar se combina con todos los usuarios que estén
        /// suscritos al evento, esos datos vendrían a ser las notificaciones que se guardan en una base de 
        /// datos no relacional. Los eventos que han iniciado contiene solamente el id del evento ya que se 
        /// necesita para cambiarlo del estado notificado(5) a procesado(4). El otro listado son las notificaciones 
        /// que se envían a los usuarios que estén dentro del grupo(evento), y cambiar el estado del evento a
        /// notificado(4), estos datos se obtienen a partir de los eventos que están a una hora de iniciar, 
        /// especificando que sean distintos para evitar que se envíe más de una notificación por grupo, ya que 
        /// el primer listado contiene repetidos debido a que se incluye los suscriptores
        /// </remarks>
        /// <param name="state"></param>
        private void SendInfo(object? state)
        {
            var task = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _eventoService = scope.ServiceProvider.GetRequiredService<IEventoService>();

                var dataEvents = _eventoService.GetNextEvent();
                var collectionsNotifications = dataEvents.Item1.ToList();
                var messageNotifications = dataEvents.Item2.ToList();
                var eventsInitialize = dataEvents.Item3.ToList();

                if (eventsInitialize.Count != 0)
                    eventsInitialize.ForEach(id => _eventoService.ChangeEventProcessed(id));

                if (collectionsNotifications.Count != 0)
                {
                    await _notificationCollection.InsertManyAsync(collectionsNotifications);
                    foreach (var x in messageNotifications)
                    {
                        _eventoService.ChangeEventNotified(long.Parse(x.Grupo));
                        await _suscriptionHub.Clients.Group(x.Grupo).SendAsync("Notification", x);
                    }
                }
            });
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
