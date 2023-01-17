using BL.Models;
using BL.Repositories;
using BL.ViewModels;

namespace BL.Services.Implements
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }
        public AccessViewModel Login(Usuario usuario)
        {
            return this.usuarioRepository.Login(usuario);
        }
    }
}
