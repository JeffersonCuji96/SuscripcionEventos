using BL.Models;
using BL.ViewModels;

namespace BL.Services
{
    public interface IUsuarioService:IGenericService<Usuario>
    {
        AccessViewModel Login(Usuario usuario);
    }
}
