using BL.Models;
using BL.ViewModels;

namespace BL.Repositories
{
    public interface IUsuarioRepository:IGenericRepository<Usuario>
    {
        AccessViewModel Login(Usuario usuario);
    }
}
