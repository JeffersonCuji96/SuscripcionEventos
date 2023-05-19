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

        /// <summary>
        /// Método para validar las fechas de inicio y fin de un evento
        /// </summary>
        /// <remarks>
        /// Se valida si las fechas de inicio y/o fin del evento coinciden con las fechas de 
        /// otros eventos creados por el mismo usuario. Devuelve un mensaje si existe alguna 
        /// coincidencia y una cadena vacía si no hay coincidencias. Antes de realizar la validación
        /// se verifica el id del evento, si es igual a cero significa que se va a validar un registro
        /// caso contrario sería una actualización. Si es una actualización se excluye el evento actual
        /// ya que coincidiría con las verificaciones por que se compara con las fechas del mismo evento.
        /// También en caso de no haber fecha final del evento como es opcional, solamente se realiza las 
        /// verificaciones correspondientes al inicio del evento
        /// </remarks>.
        /// <param name="eventoDTO"></param>
        /// <param name="idEvento"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método que obtiene los eventos junto a la cantidad de suscriptores
        /// </summary>
        /// <remarks>
        /// Se recibe la categoría como parámetro, en caso de ser 0 se retorna todos el listado de 
        /// manera predeterminada caso contrario se filtran los datos por categoria
        /// </remarks>
        /// <param name="idCategoria"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Método para obtener los eventos organizados por el usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IEnumerable<Evento> GetEventsByUser(long idUsuario)
        {
            return testContext.Eventos.Include(x => x.Categoria).Where(x => x.IdUsuario == idUsuario && x.IdEstado != 2);
        }

        /// <summary>
        /// Método para realizar la eliminación lógica del evento
        /// </summary>
        /// <param name="idEvento"></param>
        public void RemoveEvent(long idEvento)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Evento SET IdEstado = 2 WHERE Id = @id",
                new SqlParameter("@id", idEvento));
        }

        /// <summary>
        /// Método para actualizar la información de un evento
        /// </summary>
        /// <remarks>
        /// La actualización de la foto se realiza solamente si se 
        /// ha enviado un nueva imagen
        /// </remarks>
        /// <param name="evento"></param>
        /// <param name="checkImage"></param>
        public void UpdateEvent(Evento evento, bool checkImage)
        {
            testContext.Entry(evento).State = EntityState.Modified;
            testContext.Entry(evento).Property(x => x.Foto).IsModified = checkImage;
            testContext.SaveChanges();
        }

        /// <summary>
        /// Método para obtener la ruta de la foto del evento
        /// </summary>
        /// <remarks>
        /// Se utiliza un procedimiento almacenado, donde se envían el id del evento como parámetro de ingreso y 
        /// el path como parámetro de salida, se retorna la ruta en caso de existir
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para obtener el evento al que se va a suscribir un usuario
        /// </summary>
        /// <remarks>
        /// Se obtiene solo el identificador, la fecha inicial y final del evento que son
        /// necesarios para realizar la validación correspondiente de las fechas antes de
        /// que el usuario se suscriba a un evento
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método que obtiene los eventos que están a una hora de iniciar, los que han iniciado y los que se van a notificar
        /// </summary>
        /// <remarks>
        /// Se inicia desde una consulta principal donde se obtiene los eventos que coincidan con la fecha actual y 
        /// un estádo válido. Los eventos que están por iniciar se combina con todos los usuarios que estén
        /// suscritos al evento, esos datos vendrían a ser las notificaciones que se guardan en una base de 
        /// datos no relacional. Los eventos que han iniciado contiene solamente el id del evento ya que se 
        /// necesita para cambiarlo del estado notificado(5) a procesado(4). El otro listado son las notificaciones 
        /// que se envían a los usuarios que estén dentro del grupo(evento), y cambiar el estado del evento a
        /// notificado(4), estos datos se obtienen a partir de los eventos que están a una hora de iniciar, 
        /// especificando que sean distintos para evitar que se envíe más de una notificación por grupo, ya que 
        /// el primer listado contiene repetidos debido a que se incluye los suscriptores
        /// </remarks>
        /// <returns></returns>
        public (IEnumerable<NotificationViewModel>,IEnumerable<MessageViewModel>, IEnumerable<long>) GetNextEvent()
        {
            var currentDate = DateHelper.GetCurrentDate();
            var hourNotified = TimeSpan.Parse(currentDate.AddHours(1).AddSeconds(-currentDate.Second).ToString("HH:mm:ss tt"));
            var hourCurrent = TimeSpan.Parse(currentDate.AddSeconds(-currentDate.Second).ToString("HH:mm:ss tt"));

            var eventsQuery = testContext.Eventos.Where(x => x.FechaInicio == currentDate.Date && (x.IdEstado==1 || x.IdEstado==5));

            var lstEventsSuscriptors = eventsQuery
                .Where(x => x.HoraInicio == hourNotified)
                .Join(testContext.Suscripciones.Where(x=>x.IdEstado==1), e => e.Id, s => s.IdEvento, (e, s) => new { Evento = e, Suscriptor = s })
                .Select(x => new NotificationViewModel()
                {
                    IdEvento = x.Evento.Id,
                    TituloEvento = x.Evento.Titulo,
                    InicioEvento = x.Evento.FechaInicio.ToString("yyyy-MM-dd") +" "+ x.Evento.HoraInicio.ToString("hh\\:mm"),
                    IdUsuarioSuscrito = x.Suscriptor.IdUsuario,
                    Estado = 1
                });

            var lstEventsNotification = lstEventsSuscriptors
                .Select(x => new MessageViewModel()
                { 
                    Grupo=x.IdEvento.ToString(), 
                    Evento=x.TituloEvento, 
                    Inicio=x.InicioEvento 
                }).Distinct();

            var lstEventsInitialize = eventsQuery.
                Where(x => x.HoraInicio == hourCurrent)
                .Select(x => x.Id);

            return (lstEventsSuscriptors,lstEventsNotification,lstEventsInitialize);
        }

        /// <summary>
        /// Método para cambiar el estado del evento a notificado
        /// </summary>
        /// <param name="idEvento"></param>
        public void ChangeEventNotified(long idEvento)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Evento SET IdEstado = 5 WHERE Id = @id",
                new SqlParameter("@id", idEvento));
        }

        /// <summary>
        /// Método para cambiar el estado del evento a procesado
        /// </summary>
        /// <param name="idEvento"></param>
        public void ChangeEventProcessed(long idEvento)
        {
            testContext.Database.ExecuteSqlRaw("UPDATE Evento SET IdEstado = 4 WHERE Id = @id",
                new SqlParameter("@id", idEvento));
        }

        /// <summary>
        /// Método para obtener los identificadores de cada evento al que está suscrito el usuario
        /// </summary>
        /// <remarks>
        /// Solo se obtiene la información que sea de la fecha actual, para ser usado en un posible
        /// ingreso de un grupo de notificaciones, en este caso el grupo vendría a ser el evento
        /// </remarks>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public IEnumerable<long> GetEventsTodayByUser(long idUsuario)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var idsEvents = testContext.Suscripciones.Where(x => x.IdEstado == 1 && x.IdUsuario==idUsuario && x.Evento.FechaInicio==currentDate).Select(x=>x.IdEvento);
            return idsEvents;
        }

        /// <summary>
        /// Método para obtener los datos de una notificación
        /// </summary>
        /// <param name="idEvento"></param>
        /// <returns></returns>
        public EventoSuscripcionViewModel? GetEventNotification(long idEvento)
        {
            var eventNotification = testContext.Eventos.Include(p => p.Usuario.Persona).Include(c => c.Categoria)
                .Include(s => s.Suscripciones).Select(e => new EventoSuscripcionViewModel
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
                    IdCategoria = e.IdCategoria,
                    Organizador = new PersonaViewModel()
                    {
                        Id = e.Usuario.Persona.Id,
                        NombreApellido = e.Usuario.Persona.Nombre + ' ' + e.Usuario.Persona.Apellido,
                        Foto = e.Usuario.Persona.Foto
                    },
                    Suscriptores = e.Suscripciones.Count(x => x.IdEstado == 1)
                }).FirstOrDefault(x => x.Id == idEvento && x.IdEstado != 2);

            return eventNotification;
        }
    }
}