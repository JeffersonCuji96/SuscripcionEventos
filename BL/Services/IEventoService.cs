using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IEventoService:IGenericService<Evento>
    {
        string CheckDateEvent(DateTime? fechaInicio, DateTime? fechaFin, long? idUsuario);
        IEnumerable<EventoSuscripcionViewModel> GetEventsSuscriptions(int idCategoria);
    }
}
