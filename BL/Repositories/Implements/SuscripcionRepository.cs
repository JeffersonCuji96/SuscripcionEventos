using BL.Common;
using BL.DTO;
using BL.Models;
using BL.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace BL.Repositories.Implements
{
    public class SuscripcionRepository : GenericRepository<Suscripcion>, ISuscripcionRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        private readonly AppSettings appSettings;
        public SuscripcionRepository(DbSuscripcionEventosContext testContext, IOptions<AppSettings> appSettings) : base(testContext)
        {
            this.testContext = testContext;
            this.appSettings = appSettings.Value;
        }
        public long SuscribeEvent(long idSuscripcion, bool tipo)
        {
            if (tipo == false)
                testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 2 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            else testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 1 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            return idSuscripcion;
        }

        public Tuple<long,int> CheckSuscribeUser(SuscribeCheckViewModel suscribe)
        {
            var oSuscripcion = testContext.Suscripciones.FirstOrDefault(x => x.IdUsuario == suscribe.IdUsuario && x.IdEvento == suscribe.IdEvento);
            long id = oSuscripcion?.Id ?? 0;
            int estado = oSuscripcion?.IdEstado ?? 0;
            return Tuple.Create(id,estado);
        }

        public string CheckDateEventSuscription(EventCheckViewModel eventViewModel)
        {
            var lstEventsUser = new List<Suscripcion>();
            string message = "No se puede suscribir a un evento donde la fecha de inicio y/o fin coincide con fechas de otros eventos a los que se ha suscrito";
            lstEventsUser = testContext.Suscripciones.Include(x => x.Evento).Where(x => x.IdUsuario == eventViewModel.IdUsuario && x.IdEstado == 1 && x.Evento.Id != eventViewModel.Id).ToList();

            if (eventViewModel.FechaFin != null)
            {
                if (lstEventsUser.Any(x => x.Evento.FechaInicio == eventViewModel.FechaFin || x.Evento.FechaInicio == eventViewModel.FechaInicio || x.Evento.FechaFin == eventViewModel.FechaInicio || x.Evento.FechaFin == eventViewModel.FechaFin ||
                (x.Evento.FechaInicio > eventViewModel.FechaInicio && x.Evento.FechaFin < eventViewModel.FechaFin) || (eventViewModel.FechaInicio > x.Evento.FechaInicio && eventViewModel.FechaInicio < x.Evento.FechaFin) ||
                (eventViewModel.FechaInicio > x.Evento.FechaInicio && eventViewModel.FechaFin < x.Evento.FechaFin) || (eventViewModel.FechaFin > x.Evento.FechaInicio && eventViewModel.FechaFin < x.Evento.FechaFin) ||
                (x.Evento.FechaInicio > eventViewModel.FechaInicio && x.Evento.FechaInicio < eventViewModel.FechaFin)))
                    return message;
                return string.Empty;
            }
            if (lstEventsUser.Any(x => x.Evento.FechaInicio == eventViewModel.FechaInicio || x.Evento.FechaFin == eventViewModel.FechaInicio || (eventViewModel.FechaInicio > x.Evento.FechaInicio && eventViewModel.FechaInicio < x.Evento.FechaFin)))
                return message;
            return string.Empty;
        }
    }
}
