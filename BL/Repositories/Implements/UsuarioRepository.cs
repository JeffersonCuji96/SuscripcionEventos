using BL.Common;
using BL.Helpers;
using BL.Models;
using BL.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
        public bool CheckPassword(UserPasswordViewModel userPassViewModel)
        {
            string ePass = Crypto.GetSHA256(userPassViewModel.Clave);
            return testContext.Usuarios.Any(x => x.Id == userPassViewModel.Id && x.Clave == ePass);
        }
        public bool CheckEmail(string email)
        {
            string eEmail = Crypto.GetSHA256(email);
            return testContext.Usuarios.Any(x => x.Email == eEmail);
        }
        public void UpdateEmail(UserEmailViewModel userEmailViewModel)
        {
            var eEmail = Crypto.GetSHA256(userEmailViewModel.Email);
            testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET Email = @email WHERE Id = @id",
                new SqlParameter("@id", userEmailViewModel.Id),
                new SqlParameter("@email", eEmail));
        }
        public void UpdateClave(UserPasswordViewModel userPassViewModel)
        {
            var ePass = Crypto.GetSHA256(userPassViewModel.Clave);
            testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET Clave = @pass WHERE Id = @id",
                new SqlParameter("@id", userPassViewModel.Id),
                new SqlParameter("@pass", ePass));
        }

        /*
            Se genera un número pseudoaleatorio encriptado para actualizar el campo del token 
            en la entidad usuario. También se actualiza la fecha de expiración que se agregando 
            5 minutos a la fecha actual, que será el tiempo que dure el enlace de 
            recuperación que se envía al correo del usuario.
         */
        public void RecoveryAccess(UserEmailViewModel userEmailViewModel, DateTime date)
        {
            string token = Crypto.GetSHA256(Guid.NewGuid().ToString());
            var query = testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET TokenRecuperacion = @tokenRecuperacion,FechaTokenExpiracion=@fechaTokenExpiracion WHERE Id = @id",
                new SqlParameter("@id", userEmailViewModel.Id),
                new SqlParameter("@tokenRecuperacion", token),
                new SqlParameter("@fechaTokenExpiracion", date.AddMinutes(5)));

            if (query == 1)
            {
                var oMailSetting = new MailSettings()
                {
                    Path = "/auth/change-password/",
                    Subject = "Cambio de contraseña",
                    Body = "<p>Recupere el acceso a su cuenta</p><br>",
                    LinkDescription = "Click para restablecer"
                };
                MailHelper.SendEmail(userEmailViewModel.Email, token, appSettings, oMailSetting);
            }
        }
        /*
            Se obtiene la fecha de caducidad del token para compararlo con la fecha actual del servidor.
         */
        public bool CheckToken(TokenValidViewModel tokenValidViewModel, DateTime currentDate)
        {
            SqlParameter[] parameters = {
                    new SqlParameter{ ParameterName = "@token", SqlDbType=SqlDbType.VarChar,Size=100, Value = tokenValidViewModel.Token },
                    new SqlParameter{ ParameterName = "@tokenExpiracion",SqlDbType=SqlDbType.DateTime, Direction = ParameterDirection.Output }
                };
            testContext.Database.ExecuteSqlRaw("exec SPGetTokenExpiration @token, @tokenExpiracion OUTPUT", parameters);
            if (!string.IsNullOrEmpty(parameters[1].Value.ToString()))
            {
                DateTime fechaExpiracion = Convert.ToDateTime(parameters[1].Value);
                if (currentDate > fechaExpiracion)
                    return false;
                return true;
            }
            return false;
        }
    }
}
