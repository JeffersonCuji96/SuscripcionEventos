using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface IEventoRepository : IGenericRepository<Evento>
    {
        bool CheckDailyEvent(DateTime fechaInicio, long idUsuario);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
    }
}
