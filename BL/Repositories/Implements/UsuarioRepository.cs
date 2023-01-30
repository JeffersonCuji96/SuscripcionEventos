using BL.Common;
using BL.Helpers;
using BL.Models;
using BL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BL.Repositories.Implements
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly DbSuscripcionEventosContext testContext;
        private readonly AppSettings appSettings;
        public UsuarioRepository(DbSuscripcionEventosContext testContext, IOptions<AppSettings> appSettings) : base(testContext)
        {
            this.testContext = testContext;
            this.appSettings = appSettings.Value;
        }
        public AccessViewModel Login(Usuario usuario)
        {
            var oAccessViewModel = new AccessViewModel();
            var oUsuario = testContext.Usuarios.FirstOrDefault(
                x => x.Email == Crypto.GetSHA256(usuario.Email) &&
                x.Clave == Crypto.GetSHA256(usuario.Clave));
            if (oUsuario != null)
            {
                oAccessViewModel.IdUsuario = oUsuario.Id;
                oAccessViewModel.JwtToken = GenerateJwt(oUsuario.Id);
                oAccessViewModel.DaysExpireToken = appSettings.DaysExpireToken;
            }
            return oAccessViewModel;
        }
        private string GenerateJwt(long id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, id.ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(appSettings.DaysExpireToken),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public Usuario? GetUserPersonById(long id)
        {
            return testContext.Usuarios.Include(x => x.Persona).FirstOrDefault(x => x.Id == id);
        }
        public void InsertUserPerson(Usuario usuario)
        {
            usuario.Clave = Crypto.GetSHA256(usuario.Clave);
            usuario.Email = Crypto.GetSHA256(usuario.Email);
            testContext.Personas.Add(usuario.Persona);
            usuario.Id = usuario.Persona.Id;
            testContext.Usuarios.Add(usuario);
            testContext.SaveChanges();
        }
        public bool CheckPassword(string password, long id)
        {
            string ePass = Crypto.GetSHA256(password);
            return testContext.Usuarios.Any(x => x.Id == id && x.Clave == ePass);
        }
        public bool CheckEmail(string email)
        {
            string eEmail = Crypto.GetSHA256(email);
            return testContext.Usuarios.Any(x => x.Email == eEmail);
        }

    }
}
