using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using BL.Services;

namespace Api
{
    public class PopulationHostedService:IHostedService,IDisposable
    {
        private Timer? _timer;
        private readonly IHubContext<SuscriptionHub>_suscriptionHub;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IEventoService? _eventoService;
        public PopulationHostedService(IHubContext<SuscriptionHub> _suscriptionHub,
            IServiceScopeFactory _serviceScopeFactory)
        {
            this._suscriptionHub = _suscriptionHub;
            this._serviceScopeFactory = _serviceScopeFactory;
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
            var task = Task.Run(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                _eventoService = scope.ServiceProvider.GetRequiredService<IEventoService>();

                var nextEvents = _eventoService.GetNextEvent().Item2.ToList();
                if (nextEvents != null)
                {
                    foreach (var x in nextEvents)
                    {
                        _eventoService.ChangeEventFinalize(long.Parse(x.Grupo));
                        _suscriptionHub.Clients.Group(x.Grupo).SendAsync("Notification", x);
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
