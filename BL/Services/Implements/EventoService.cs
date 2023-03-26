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
        public bool CheckDailyEvent(DateTime fechaInicio, long idUsuario)
        {
            return eventoRepository.CheckDailyEvent(fechaInicio, idUsuario);
        }
        public IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria)
        {
            return eventoRepository.GetEventsSuscriptions(idCategoria);
        }
    }
}
