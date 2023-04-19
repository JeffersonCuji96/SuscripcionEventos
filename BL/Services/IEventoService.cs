using BL.DTO;
using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IEventoService:IGenericService<Evento>
    {
        string CheckDateEvent(EventoDTO eventoDTO, long idEvento);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
        IEnumerable<Evento> GetEventsByUser(long idUsuario);
        void RemoveEvent(long idEvento);
        void UpdateEvent(Evento evento, bool checkImage);
        string GetPathPhoto(long id);
    }
}
