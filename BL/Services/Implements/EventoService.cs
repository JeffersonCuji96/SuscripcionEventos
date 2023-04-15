using BL.Models;
using BL.Repositories;
using BL.ViewModels;

namespace BL.Services.Implements
{
    public class EventoService : GenericService<Evento>, IEventoService
    {
        private readonly IEventoRepository eventoRepository;
        public EventoService(IEventoRepository eventoRepository) : base(eventoRepository)
        {
            this.eventoRepository = eventoRepository;
        }
        public string CheckDateEvent(DateTime? fechaInicio, DateTime? fechaFin, long? idUsuario)
        {
            return eventoRepository.CheckDateEvent(fechaInicio, fechaFin, idUsuario);
        }
        public IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria)
        {
            return eventoRepository.GetEventsSuscriptions(idCategoria);
        }
        public IEnumerable<Evento> GetEventsByUser(long idUsuario)
        {
            return eventoRepository.GetEventsByUser(idUsuario);
        }
    }
}
