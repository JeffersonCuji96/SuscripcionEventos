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
        /// Se inicia desde una consulta principal donde se obtienen los eventos que coincidan con la fecha actual y 
        /// un estádo válido. Los eventos que están por iniciar se combinan con todos los usuarios que estén
        /// suscritos al evento, esos datos vendrían a ser las notificaciones que se guardan en una base de 
        /// datos no relacional. Los eventos que están por inician o han iniciado contienen solamente el id del evento ya que se 
        /// necesita para cambiarlo del estado activo(1) a notificado(5) y después a procesado(4) según corresponda. Además se hace un
        /// seguimiento del cambio de los estados, para notificarlos y realizar alguna operación de lado del cliente.
        /// </remarks>
        /// <param name="state"></param>
        private void SendInfo(object? state)
        {
            var task = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _eventoService = scope.ServiceProvider.GetRequiredService<IEventoService>();

                var dataEvents = _eventoService.GetNextEvent();
                var collectionsSuscriptions = dataEvents.Item1.ToList();
                var suscriptionsNotifications = dataEvents.Item2.ToList();
                var eventsProcessed = dataEvents.Item3[0];
                var eventsNotified = dataEvents.Item3[1];

                if (eventsProcessed.Length != 0) 
                { 
                    foreach(var x in eventsProcessed)
                    {
                        _eventoService.ChangeEventProcessed(long.Parse(x));
                        await _suscriptionHub.Clients.Group(x).SendAsync("Processed", true);
                    }
                }

                if (eventsNotified.Length != 0)
                {
                    foreach (var x in eventsNotified)
                    {
                        _eventoService.ChangeEventNotified(long.Parse(x));
                        await _suscriptionHub.Clients.Group(x).SendAsync("Notified", true);
                    }
                }

                if (collectionsSuscriptions.Count != 0)
                {
                    await _notificationCollection.InsertManyAsync(collectionsSuscriptions);
                    foreach (var x in suscriptionsNotifications)
                    {
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
