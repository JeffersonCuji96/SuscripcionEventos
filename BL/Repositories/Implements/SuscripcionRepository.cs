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

        /// <summary>
        /// Método para suscribirse o desuscribirse de un evento
        /// </summary>
        /// <remarks>
        /// El estado 1 vendría a ser una suscripción y el estado 2 la desuscripción
        /// </remarks>
        /// <param name="idSuscripcion"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public long SuscribeEvent(long idSuscripcion, bool tipo)
        {
            if (tipo == false)
                testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 2 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            else testContext.Database.ExecuteSqlRaw("UPDATE Suscripcion SET IdEstado = 1 WHERE Id = @id", new SqlParameter("@id", idSuscripcion));
            return idSuscripcion;
        }

        /// <summary>
        /// Método para verificar que el usuario está suscrito a un evento
        /// </summary>
        /// <param name="suscribe"></param>
        /// <returns></returns>
        public Tuple<long,int> CheckSuscribeUser(SuscribeCheckViewModel suscribe)
        {
            var oSuscripcion = testContext.Suscripciones.FirstOrDefault(x => x.IdUsuario == suscribe.IdUsuario && x.IdEvento == suscribe.IdEvento);
            long id = oSuscripcion?.Id ?? 0;
            int estado = oSuscripcion?.IdEstado ?? 0;
            return Tuple.Create(id,estado);
        }

        /// <summary>
        /// Método para validar la suscripción de un usuario
        /// </summary>
        /// <remarks>
        /// Se verifica que las fechas del evento al que el usuario se quiere suscribir
        /// no coincidan con las fechas de otras suscripciones
        /// </remarks>
        /// <param name="eventViewModel"></param>
        /// <returns></returns>
        public string CheckDateEventSuscription(EventCheckViewModel eventViewModel)
        {
            var lstEventsUser = new List<Evento>();
            string message = "No se puede suscribir a un evento donde la fecha de inicio y/o fin coincide con fechas de otros eventos a los que se ha suscrito";
            lstEventsUser = testContext.Suscripciones.Include(x => x.Evento)
                .Where(x => x.IdEstado==1 && x.IdUsuario == eventViewModel.IdUsuario)
                .Select(x=>x.Evento)
                .ToList();

            if (lstEventsUser.Count > 0)
            {
                if (eventViewModel.FechaFin != null)
                {
                    if (lstEventsUser.Any(x =>
                    (eventViewModel.FechaInicio == x.FechaInicio && eventViewModel.HoraInicio == x.HoraInicio) ||
                    (eventViewModel.FechaFin == x.FechaFin && eventViewModel.HoraFin == x.HoraFin) ||
                    (eventViewModel.FechaFin == x.FechaInicio && eventViewModel.HoraFin == x.HoraInicio) ||
                    (eventViewModel.FechaInicio == x.FechaFin && eventViewModel.HoraInicio == x.HoraFin) ||
                    (x.FechaInicio==eventViewModel.FechaInicio && x.FechaFin==null && x.HoraInicio> eventViewModel.HoraInicio && x.HoraInicio < eventViewModel.HoraFin) ||
                    (eventViewModel.FechaInicio >= x.FechaInicio && eventViewModel.HoraInicio > x.HoraInicio && eventViewModel.FechaFin <= x.FechaFin && eventViewModel.HoraFin < x.HoraFin) ||
                    (eventViewModel.FechaInicio >= x.FechaInicio && eventViewModel.HoraInicio > x.HoraInicio && eventViewModel.FechaInicio <= x.FechaFin && eventViewModel.HoraInicio < x.HoraFin) ||
                    (eventViewModel.FechaFin >= x.FechaInicio && eventViewModel.HoraFin > x.HoraInicio && eventViewModel.FechaFin <= x.FechaFin && eventViewModel.HoraFin < x.HoraFin) ||
                    (x.FechaInicio >= eventViewModel.FechaInicio && x.FechaFin <= eventViewModel.FechaFin && x.HoraInicio > eventViewModel.HoraInicio && x.HoraFin < eventViewModel.HoraFin)
                    ))
                        return message;
                }
                else
                {
                    if (lstEventsUser.Any(x =>
                    (eventViewModel.FechaInicio == x.FechaInicio && eventViewModel.HoraInicio == x.HoraInicio) ||
                    (eventViewModel.FechaInicio == x.FechaInicio && eventViewModel.HoraInicio > x.HoraInicio && eventViewModel.HoraInicio<x.HoraFin) ||
                    (eventViewModel.FechaInicio > x.FechaInicio && eventViewModel.FechaInicio < x.FechaFin) ||
                    (eventViewModel.FechaInicio == x.FechaFin && eventViewModel.HoraInicio == x.HoraFin)
                    ))
                        return message;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Método par obtener un listado de las suscripciones de un usuario
        /// </summary>
        /// <remarks>
        /// Junto a las suscripciones se agrega la cantidad de suscriptores que tiene cada evento 
        /// al que esté suscrito el usuario para posteriormente usar esa información en el detalle
        /// </remarks>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IEnumerable<EventoSuscripcionViewModel> GetSuscriptionsByUser(long idUsuario)
        {
            var lstEvents = testContext.Suscripciones.Include(x => x.Evento).ThenInclude(x => x.Categoria)
                .Include(x => x.Usuario.Persona).Where(x => x.IdUsuario == idUsuario && x.IdEstado == 1 && x.Evento.IdEstado != 2)
                .Select(e => new EventoSuscripcionViewModel
                {
                    Id = e.Evento.Id,
                    Titulo = e.Evento.Titulo,
                    FechaInicio = e.Evento.FechaInicio,
                    HoraInicio = e.Evento.HoraInicio,
                    FechaFin = e.Evento.FechaFin,
                    HoraFin = e.Evento.HoraFin,
                    Ubicacion = e.Evento.Ubicacion,
                    InformacionAdicional = e.Evento.InformacionAdicional,
                    Foto = e.Evento.Foto,
                    Categoria = e.Evento.Categoria.Descripcion,
                    IdEstado= e.Evento.IdEstado,
                    IdCategoria=e.Evento.IdCategoria,
                    Organizador = new PersonaViewModel()
                    {
                        Id = e.Evento.Usuario.Persona.Id,
                        NombreApellido = e.Evento.Usuario.Persona.Nombre + ' ' + e.Evento.Usuario.Persona.Apellido,
                        Foto = e.Evento.Usuario.Persona.Foto
                    },
                    Suscriptores = e.Evento.Suscripciones.Count(x => x.IdEstado == 1)
                });
            return lstEvents;
        }
    }
}
