using BL.DTO;
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
        public string CheckDateEvent(EventoDTO eventoDTO, long idEvento)
        {
            return eventoRepository.CheckDateEvent(eventoDTO, idEvento);
        }
        public IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria)
        {
            return eventoRepository.GetEventsSuscriptions(idCategoria);
        }
        public IEnumerable<Evento> GetEventsByUser(long idUsuario)
        {
            return eventoRepository.GetEventsByUser(idUsuario);
        }
        public void RemoveEvent(long idEvento)
        {
            eventoRepository.RemoveEvent(idEvento);
        }
        public void UpdateEvent(Evento evento, bool checkImage)
        {
            eventoRepository.UpdateEvent(evento, checkImage);
        }

        public string GetPathPhoto(long id)
        {
            return eventoRepository.GetPathPhoto(id);
        }
        public EventCheckViewModel GetDataEventCheck(long id)
        {
            return eventoRepository.GetDataEventCheck(id);
        }
        public (IEnumerable<NotificationViewModel>, IEnumerable<MessageViewModel>, IEnumerable<long>) GetNextEvent()
        {
            return eventoRepository.GetNextEvent();
        }
        public void ChangeEventNotified(long idEvento)
        {
            eventoRepository.ChangeEventNotified(idEvento);
        }
        public IEnumerable<long> GetEventsTodayByUser(long idUsuario)
        {
            return eventoRepository.GetEventsTodayByUser(idUsuario);
        }
        public EventoSuscripcionViewModel? GetEventNotification(long idEvento)
        {
            return eventoRepository.GetEventNotification(idEvento);
        }
        public void ChangeEventProcessed(long idEvento)
        {
            eventoRepository.ChangeEventProcessed(idEvento);
        }
    }
}
