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
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendInfo, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
            return Task.CompletedTask;

        }

        /*Se crea un hilo para el envío de notificaciones, el id del evento vendría
          a ser el id del grupo, posteriormente se cambia el estado del evento a finalizado*/
        private void SendInfo(object? state)
        {
            var task = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _eventoService = scope.ServiceProvider.GetRequiredService<IEventoService>();

                var tupleData = _eventoService.GetNextEvent();
                var collection = tupleData.Item1.ToList();
                var nextEvents = tupleData.Item2.ToList();

                if (nextEvents != null)
                {
                    await _notificationCollection.InsertManyAsync(collection);
                    foreach (var x in nextEvents)
                    {
                        _eventoService.ChangeEventFinalize(long.Parse(x.Grupo));
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
