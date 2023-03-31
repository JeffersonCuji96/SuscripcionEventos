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
        public bool CheckDailyEvent(DateTime fechaInicio, long idUsuario)
        {
            var oEventoLast = testContext.Eventos.OrderBy(x => x.Id).LastOrDefault(x => x.IdUsuario == idUsuario);
            if (oEventoLast == null)
                return true;
            if (oEventoLast.FechaFin != null)
                return fechaInicio > oEventoLast.FechaFin;
            return fechaInicio > oEventoLast.FechaInicio;
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