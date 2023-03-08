using BL.Common;
using BL.Helpers;
using BL.Models;
using BL.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
        public Tuple<AccessViewModel, int> Login(Usuario usuario)
        {
            int state = 0;
            var oAccessViewModel = new AccessViewModel();
            var oUsuario = testContext.Usuarios.Include(x=>x.Persona).FirstOrDefault(
                x => x.Email == Crypto.GetSHA256(usuario.Email) &&
                x.Clave == Crypto.GetSHA256(usuario.Clave));

            if (oUsuario!=null)
            {
                state = oUsuario.IdEstado;
                if (oUsuario.IdEstado == 1)
                {
                    oAccessViewModel.IdUsuario = oUsuario.Id;
                    oAccessViewModel.JwtToken = GenerateJwt(oUsuario.Id);
                    oAccessViewModel.DaysExpireToken = appSettings.DaysExpireToken;
                    oAccessViewModel.FullName = $"{oUsuario.Persona.Nombre +' '+ oUsuario.Persona.Apellido}";
                }
            }
            return Tuple.Create(oAccessViewModel,state);
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
        public void InsertUserPerson(Usuario usuario)
        {
            string token = Crypto.GetSHA256(Guid.NewGuid().ToString());
            string emailOrigin = usuario.Email;
            usuario.Clave = Crypto.GetSHA256(usuario.Clave);
            usuario.Email = Crypto.GetSHA256(usuario.Email);

            testContext.Personas.Add(usuario.Persona);
            usuario.Id = usuario.Persona.Id;
            usuario.IdEstado = 3;
            usuario.Token = token;
            testContext.Usuarios.Add(usuario);
            testContext.SaveChanges();

            var oMailSetting = new MailSettings()
            {
                Path = "/auth/confirm-email/",
                Subject = "Habilitación de cuenta",
                Body = "<p>Confirme su email para acceder a su cuenta</p><br>",
                LinkDescription = "Click aquí para confirmar"
            };
            MailHelper.SendEmail(emailOrigin, token, appSettings, oMailSetting);
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
            string token = Crypto.GetSHA256(Guid.NewGuid().ToString());
            var eEmail = Crypto.GetSHA256(userEmailViewModel.Email);
            testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET Email = @email, IdEstado = 3, Token=@token WHERE Id = @id",
                new SqlParameter("@id", userEmailViewModel.Id),
                new SqlParameter("@email", eEmail),
                new SqlParameter("@token", token));

            var oMailSetting = new MailSettings()
            {
                Path = "/auth/confirm-email/",
                Subject = "Cambio de email",
                Body = "<p>Confirme su email para acceder a su cuenta</p><br>",
                LinkDescription = "Click aquí para confirmar"
            };
            MailHelper.SendEmail(userEmailViewModel.Email, token, appSettings, oMailSetting);
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
            var query = testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET Token = @token,FechaTokenExpiracion=@fechaExpiracion WHERE Id = @id",
                new SqlParameter("@id", userEmailViewModel.Id),
                new SqlParameter("@token", token),
                new SqlParameter("@fechaExpiracion", date.AddMinutes(5)));

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
                    new SqlParameter{ ParameterName = "@fechaExpiracion",SqlDbType=SqlDbType.DateTime, Direction = ParameterDirection.Output }
                };
            testContext.Database.ExecuteSqlRaw("exec SPGetTokenExpiration @token, @fechaExpiracion OUTPUT", parameters);
            if (!string.IsNullOrEmpty(parameters[1].Value.ToString()))
            {
                DateTime fechaExpiracion = Convert.ToDateTime(parameters[1].Value);
                if (currentDate > fechaExpiracion)
                    return false;
                return true;
            }
            return false;
        }
        public bool ChangeClave(TokenPasswordViewModel tokenPassViewModel)
        {
            var oUser = testContext.Usuarios.FirstOrDefault(x => x.Token == tokenPassViewModel.Token);
            if (oUser != null)
            {
                oUser.Clave = Crypto.GetSHA256(tokenPassViewModel.Clave);
                oUser.Token = null;
                oUser.FechaTokenExpiracion = null;
                testContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ConfirmEmail(TokenValidViewModel tokenValidViewModel)
        {
            var oUser = testContext.Usuarios.FirstOrDefault(x => x.Token == tokenValidViewModel.Token && x.IdEstado==3);
            if (oUser != null)
            {
                oUser.Token = null;
                oUser.IdEstado = 1;
                testContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool VerifyStatusUser(long id)
        {
            return testContext.Usuarios.Any(x=>x.Id==id && x.IdEstado==1);
        }
    }
}
