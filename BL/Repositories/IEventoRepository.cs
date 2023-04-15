using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface IEventoRepository : IGenericRepository<Evento>
    {
        string CheckDateEvent(DateTime? fechaInicio,DateTime? fechaFin, long? idUsuario);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
        IEnumerable<Evento> GetEventsByUser(long idUsuario);
    }
}
