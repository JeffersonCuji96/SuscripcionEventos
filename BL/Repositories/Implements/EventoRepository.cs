using BL.Models;

namespace BL.Repositories.Implements
{
    public class EventoRepository : GenericRepository<Evento>, IEventoRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        public EventoRepository(DbSuscripcionEventosContext testContext) : base(testContext)
        {
            this.testContext = testContext;
        }
        public bool CheckDailyEvent(DateTime fechaInicio, long idUsuario)
        {
            var oEventoLast = testContext.Eventos.OrderBy(x=>x.Id).LastOrDefault(x=>x.IdUsuario==idUsuario);
            if (oEventoLast == null)
                return true;
            if (oEventoLast.FechaFin != null)
                return fechaInicio > oEventoLast.FechaFin;
            return fechaInicio > oEventoLast.FechaInicio;
        }
    }
}
