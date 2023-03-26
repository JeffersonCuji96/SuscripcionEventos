using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IEventoService:IGenericService<Evento>
    {
        bool CheckDailyEvent(DateTime fechaInicio, long idUsuario);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
    }
}
