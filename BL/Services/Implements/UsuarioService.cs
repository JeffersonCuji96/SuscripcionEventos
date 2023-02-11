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
        public Tuple<AccessViewModel, int> Login(Usuario usuario)
        {
            return this.usuarioRepository.Login(usuario);
        }
        public Usuario? GetUserPersonById(long id)
        {
            return this.usuarioRepository.GetUserPersonById(id);
        }
        public void InsertUserPerson(Usuario usuario)
        {
            usuarioRepository.InsertUserPerson(usuario);
        }
        public bool CheckPassword(UserPasswordViewModel userPassViewModel)
        {
            return usuarioRepository.CheckPassword(userPassViewModel);
        }
        public bool CheckEmail(string email)
        {
            return usuarioRepository.CheckEmail(email);
        }
        public void UpdateEmail(UserEmailViewModel userEmailViewModel)
        {
            usuarioRepository.UpdateEmail(userEmailViewModel);
        }
        public void UpdateClave(UserPasswordViewModel userPassViewModel)
        {
            usuarioRepository.UpdateClave(userPassViewModel);
        }
        public void RecoveryAccess(UserEmailViewModel userEmailViewModel, DateTime date)
        {
            usuarioRepository.RecoveryAccess(userEmailViewModel, date);
        }
        public bool CheckToken(TokenValidViewModel tokenValidViewModel, DateTime currentDate)
        {
            return usuarioRepository.CheckToken(tokenValidViewModel, currentDate);
        }
        public bool ChangeClave(TokenPasswordViewModel tokenPassViewModel)
        {
            return usuarioRepository.ChangeClave(tokenPassViewModel);
        }
        public bool ConfirmEmail(TokenValidViewModel tokenValidViewModel)
        {
            return usuarioRepository.ConfirmEmail(tokenValidViewModel);
        }
        public bool VerifyStatusUser(long id)
        {
            return usuarioRepository.VerifyStatusUser(id);
        }
    }
}
