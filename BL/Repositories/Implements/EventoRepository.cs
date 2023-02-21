using BL.Common;
using BL.Models;
using Microsoft.Extensions.Options;

namespace BL.Repositories.Implements
{
    public class EventoRepository : GenericRepository<Evento>, IEventoRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        private readonly AppSettings appSettings;
        public EventoRepository(DbSuscripcionEventosContext testContext, IOptions<AppSettings> appSettings) : base(testContext)
        {
            this.testContext = testContext;
            this.appSettings = appSettings.Value;
        }
    }
}
