using BL.Models;

namespace BL.Services
{
    public interface IEventoService:IGenericService<Evento>
    {
        bool CheckDailyEvent(DateTime fechaInicio, long idUsuario);
    }
}
