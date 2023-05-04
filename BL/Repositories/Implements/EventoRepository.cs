using BL.DTO;
using BL.Helpers;
using BL.Models;
using BL.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        public string CheckDateEvent(EventoDTO eventoDTO, long idEvento)
        {
            var lstEventsUser = new List<Evento>();
            string message = "La fecha de inicio y/o fin no debe coincidir con fechas de otros eventos creados por el mismo usuario";
            
            if (idEvento == 0) 
                lstEventsUser = testContext.Eventos.Where(x => x.IdUsuario == eventoDTO.IdUsuario && x.IdEstado == 1).ToList();
            else lstEventsUser = testContext.Eventos.Where(x => x.Id != idEvento && x.IdUsuario == eventoDTO.IdUsuario && x.IdEstado == 1).ToList();

            if (eventoDTO.FechaFin != null)
            {
                if (lstEventsUser.Any(x => x.FechaInicio == eventoDTO.FechaFin || x.FechaInicio == eventoDTO.FechaInicio || x.FechaFin == eventoDTO.FechaInicio || x.FechaFin == eventoDTO.FechaFin ||
                (x.FechaInicio > eventoDTO.FechaInicio && x.FechaFin < eventoDTO.FechaFin) || (eventoDTO.FechaInicio > x.FechaInicio && eventoDTO.FechaInicio < x.FechaFin) ||
                (eventoDTO.FechaInicio > x.FechaInicio && eventoDTO.FechaFin < x.FechaFin) || (eventoDTO.FechaFin > x.FechaInicio && eventoDTO.FechaFin < x.FechaFin) ||
                (x.FechaInicio > eventoDTO.FechaInicio && x.FechaInicio < eventoDTO.FechaFin)))
                    return message;
                return string.Empty;
            }
            if (lstEventsUser.Any(x => x.FechaInicio == eventoDTO.FechaInicio || x.FechaFin == eventoDTO.FechaInicio || (eventoDTO.FechaInicio > x.FechaInicio && eventoDTO.FechaInicio < x.FechaFin)))
                return message;
            return string.Empty;
        }
        public IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria)
        {
            var lstEventSuscriptions = testContext.Eventos.Include(p => p.Usuario.Persona).Include(c => c.Categoria)
                .Include(s => s.Suscripciones).Where(x => x.IdEstado == 1 && x.Usuario.IdEstado == 1 && (idCategoria == 0 || x.IdCategoria == idCategoria))
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
                    IdEstado = e.IdEstado,
                    IdCategoria=e.IdCategoria,
                    Organizador = new PersonaViewModel()
                    {
                        Id = e.Usuario.Persona.Id,
                        NombreApellido = e.Usuario.Persona.Nombre + ' ' + e.Usuario.Persona.Apellido,
                        Foto = e.Usuario.Persona.Foto
                    },
                    Suscriptores = e.Suscripciones.Count(x=>x.IdEstado==1)
                });
            return lstEventSuscriptions.ToList();
        }
        public IEnumerable<Evento> GetEventsByUser(long idUsuario)
        {
            return testContext.Eventos.Include(x => x.Categoria).Where(x => x.IdUsuario == idUsuario && x.IdEstado != 2);
        }
        public void RemoveEvent(long idEvento)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Evento SET IdEstado = 2 WHERE Id = @id",
                new SqlParameter("@id", idEvento));
        }
        public void UpdateEvent(Evento evento, bool checkImage)
        {
            testContext.Entry(evento).State = EntityState.Modified;
            testContext.Entry(evento).Property(x => x.Foto).IsModified = checkImage;
            testContext.SaveChanges();
        }
        public string GetPathPhoto(long id)
        {
            SqlParameter[] parameters = {
                new SqlParameter{ ParameterName = "@id", SqlDbType=SqlDbType.BigInt, Value = id },
                new SqlParameter{ ParameterName = "@path",SqlDbType=SqlDbType.VarChar,Size=200, Direction = ParameterDirection.Output }
            };
            testContext.Database.ExecuteSqlRaw("exec SPGetPathPhotoEvent @id, @path OUTPUT", parameters);
            string? path = parameters[1].Value.ToString();
            return path ?? string.Empty;
        }
        public EventCheckViewModel GetDataEventCheck(long id)
        {
            var dataEventCheck = testContext.Eventos.FromSqlRaw("exec SPGetDataEventCheck @id", new SqlParameter("@id", id))
                .AsEnumerable().Select(x=>new EventCheckViewModel{
                    Id=x.Id,
                    FechaInicio=x.FechaInicio,
                    FechaFin=x.FechaFin
                }).First();
            return dataEventCheck;
        }

        /*Método que obtiene los eventos una hora antes de su inicio, se combina con todos los usuarios que estén suscritos al evento. Se retorna 
         *una tupla por que el primer listado de notificaciones se va a guardar en una base de datos no relacionañ y el otro listado contiene solo 
         *el id del evento que sería el grupo donde se envía la notificación, el título y la hora de inicio. Esto para evitar que se envíe más 
         *de una notificación por grupo, ya que el primer listado contiene repetidos debido a que se incluye los suscriptores*/
        public (IEnumerable<NotificationViewModel>,IEnumerable<MessageViewModel>) GetNextEvent()
        {
            var currentDate = DateHelper.GetCurrentDate();
            var hora = TimeSpan.Parse(currentDate.AddHours(1).AddSeconds(-currentDate.Second).ToString("HH:mm:ss tt"));
            var lstEventsSuscriptors = testContext.Eventos
                .Where(x => x.IdEstado == 1 && x.FechaInicio == currentDate.Date && x.HoraInicio == hora)
                .Join(testContext.Suscripciones.Where(x=>x.IdEstado==1), e => e.Id, s => s.IdEvento, (e, s) => new { Evento = e, Suscriptor = s })
                .Select(x => new NotificationViewModel()
                {
                    IdEvento = x.Evento.Id.ToString(),
                    TituloEvento = x.Evento.Titulo,
                    InicioEvento = x.Evento.HoraInicio.ToString("hh\\:mm"),
                    IdUsuarioSuscrito = x.Suscriptor.IdUsuario,
                    Estado = 1
                });
            var lstEventsNotification = lstEventsSuscriptors
                .Select(x => new MessageViewModel()
                { 
                    Grupo=x.IdEvento, 
                    Evento=x.TituloEvento, 
                    Inicio=x.InicioEvento 
                }).Distinct();
            return (lstEventsSuscriptors,lstEventsNotification);
        }
        public void ChangeEventFinalize(long idEvento)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Evento SET IdEstado = 4 WHERE Id = @id",
                new SqlParameter("@id", idEvento));
        }
        public IEnumerable<long> GetEventsTodayByUser(long idUsuario)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var idsEvents = testContext.Suscripciones.Where(x => x.IdEstado == 1 && x.IdUsuario==idUsuario && x.Evento.FechaInicio==currentDate).Select(x=>x.IdEvento);
            return idsEvents;
        }
    }
}