using BL.Helpers;
using BL.Models;
using BL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BL.Repositories.Implements
{
    public class EventoRepository : GenericRepository<Evento>, IEventoRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        public EventoRepository(DbSuscripcionEventosContext testContext) : base(testContext)
        {
            this.testContext = testContext;
        }
        public string CheckDateEvent(DateTime? fechaInicio, DateTime? fechaFin, long? idUsuario)
        {
            string message = "La fecha de inicio y/o fin no debe coincidir con fechas de otros eventos creado por el mismo usuario";
            var lstEventsUser = testContext.Eventos.Where(x => x.IdUsuario == idUsuario).ToList();
            if (fechaFin != null)
            {
                if (lstEventsUser.Any(x => x.FechaInicio == fechaFin || x.FechaInicio == fechaInicio || x.FechaFin == fechaInicio || x.FechaFin == fechaFin ||
                (x.FechaInicio > fechaInicio && x.FechaFin < fechaFin) || (fechaInicio > x.FechaInicio && fechaInicio < x.FechaFin) ||
                (fechaInicio > x.FechaInicio && fechaFin < x.FechaFin) || (fechaFin > x.FechaInicio && fechaFin < x.FechaFin) ||
                (x.FechaInicio > fechaInicio && x.FechaInicio < fechaFin)))
                    return message;
                return string.Empty;
            }
            if (lstEventsUser.Any(x => x.FechaInicio == fechaInicio || x.FechaFin == fechaInicio || (fechaInicio > x.FechaInicio && fechaInicio < x.FechaFin)))
                return message;
            return string.Empty;
        }
        public IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria)
        {
            var lstEventSuscriptions = testContext.Eventos.Include(p => p.Usuario.Persona).Include(c => c.Categoria)
                .Include(s => s.Suscripciones).ThenInclude(sb => sb.Usuario.Persona)
                .Where(x => idCategoria == 0 || x.IdCategoria == idCategoria && x.IdEstado == 1 && x.Usuario.IdEstado == 1)
                .Select(e => new EventoSuscripcionViewModel
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    FechaInicio = e.FechaInicio,
                    HoraInicio = e.HoraInicio,
                    FechaFin = e.FechaFin,
                    HoraFin = e.HoraFin,
                    Ubicacion = e.Ubicacion,
                    InformacionAdicional = e.InformacionAdicional,
                    Foto = e.Foto,
                    Categoria = e.Categoria.Descripcion,
                    Organizador = new PersonaViewModel()
                    {
                        Id = e.Usuario.Persona.Id,
                        NombreApellido = e.Usuario.Persona.Nombre + ' ' + e.Usuario.Persona.Apellido,
                        Foto = e.Usuario.Persona.Foto
                    },
                    Suscriptores = e.Suscripciones.Count()
                });
            return lstEventSuscriptions.ToList();
        }
    }
}