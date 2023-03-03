using BL.Models;

namespace BL.Repositories
{
    public interface IEventoRepository : IGenericRepository<Evento>
    {
        bool CheckDailyEvent(DateTime fechaInicio, long idUsuario);
    }
}
