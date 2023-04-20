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
                .Include(s => s.Suscripciones).ThenInclude(sb => sb.Usuario.Persona)
                .Where(x => x.IdEstado == 1 && x.Usuario.IdEstado == 1 && (idCategoria == 0 || x.IdCategoria == idCategoria))
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
    }
}