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

        /// <summary>
        /// Método para autenticar el usuario
        /// </summary>
        /// <remarks>
        /// En caso de ser válidas las credenciales del usuario, se verifica si la cuenta
        /// está activa, después se genera el token y se retorna los datos de identificación
        /// </remarks>
        /// <param name="usuario"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para generar un token 
        /// </summary>
        /// <remarks>
        /// Se genera un token JWT que incluye el identificador de usuario como reclamación y 
        /// está firmado con una clave secreta. El token se utiliza para autenticar y autorizar 
        /// a los usuarios en la aplicación o servicio
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para registrar a un usuario y enviar un email de confirmación
        /// </summary>
        /// <remarks>
        /// Desués del registro, se envía al usuario un correo de confirmación junto con el 
        /// token de autenticación para que pueda acceder a su cuenta
        /// </remarks>
        /// <param name="usuario"></param>
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

        /// <summary>
        /// Método para verificar la clave actual del usuario
        /// </summary>
        /// <param name="userPassViewModel"></param>
        /// <returns></returns>
        public bool CheckPassword(UserPasswordViewModel userPassViewModel)
        {
            string ePass = Crypto.GetSHA256(userPassViewModel.Clave);
            return testContext.Usuarios.Any(x => x.Id == userPassViewModel.Id && x.Clave == ePass);
        }

        /// <summary>
        /// Método para verificar si existe el email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckEmail(string email)
        {
            string eEmail = Crypto.GetSHA256(email);
            return testContext.Usuarios.Any(x => x.Email == eEmail);
        }

        /// <summary>
        /// Método para actualizar un email y enviar un correo de confirmación
        /// </summary>
        /// <param name="userEmailViewModel"></param>
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

        /// <summary>
        /// Método para actualizar la clave actual del usuario
        /// </summary>
        /// <param name="userPassViewModel"></param>
        public void UpdateClave(UserPasswordViewModel userPassViewModel)
        {
            var ePass = Crypto.GetSHA256(userPassViewModel.Clave);
            testContext.Database.ExecuteSqlRaw("UPDATE Usuario SET Clave = @pass WHERE Id = @id",
                new SqlParameter("@id", userPassViewModel.Id),
                new SqlParameter("@pass", ePass));
        }

        /// <summary>
        /// Método para recuperar el acceso a la cuenta del usuario
        /// </summary>
        /// <remarks>
        /// Se genera un número pseudoaleatorio encriptado para actualizar el campo del token 
        /// en la entidad usuario.También se actualiza la fecha de expiración que se agregando
        /// 5 minutos a la fecha actual, que será el tiempo que dure el enlace de
        /// recuperación que se envía al correo del usuario.
        /// </remarks>
        /// <param name="userEmailViewModel"></param>
        /// <param name="date"></param>
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
        /// <summary>
        /// Método para verificar que el token sea válido
        /// </summary>
        /// <remarks>
        /// Se obtiene la fecha de caducidad del token para compararlo con la fecha actual del servidor.
        /// </remarks>
        /// <param name="tokenValidViewModel"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para cambiar la clave después de la solicitud de recuperación de la cuenta
        /// </summary>
        /// <param name="tokenPassViewModel"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para confirmar la cuenta a través del email del usuario
        /// </summary>
        /// <param name="tokenValidViewModel"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para verificar el estado del usuario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool VerifyStatusUser(long id)
        {
            return testContext.Usuarios.Any(x=>x.Id==id && x.IdEstado==1);
        }
    }
}
