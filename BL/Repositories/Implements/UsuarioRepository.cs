using BL.Common;
using BL.Helpers;
using BL.Models;
using BL.ViewModels;
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

    }
}
