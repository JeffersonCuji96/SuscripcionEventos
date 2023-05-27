using BL.DTO;
using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface IEventoRepository : IGenericRepository<Evento>
    {
        string CheckDateEvent(EventoDTO eventoDTO, long idEvento);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
        IEnumerable<Evento> GetEventsByUser(long idUsuario);
        void RemoveEvent(long idEvento);
        void UpdateEvent(Evento evento, bool checkImage);
        string GetPathPhoto(long id);
        EventCheckViewModel GetDataEventCheck(long id);
        (IEnumerable<NotificationViewModel>, IEnumerable<MessageViewModel>, List<string[]>) GetNextEvent();
        void ChangeEventNotified(long idEvento);
        IEnumerable<long> GetSuscriptionsTodayByUser(long idUsuario);
        EventoSuscripcionViewModel? GetEventNotification(long idEvento);
        void ChangeEventProcessed(long idEvento);
        IEnumerable<long> GetEventsTodayByUser(long idUsuario);
        int GetStatusEvent(long idEvento);
        bool CheckEventOrganizer(OrganizerCheckViewModel organizerCheckViewModel);
    }
}
