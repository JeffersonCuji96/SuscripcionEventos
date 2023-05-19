using Api.Filters;
using Api.Helpers;
using AutoMapper;
using BL.DTO;
using BL.Models;
using BL.Services;
using BL.Services.Implements;
using BL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BL.Helpers;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUsuarioService userService;
        private readonly IPersonaService personService;
        private readonly IWebHostEnvironment hostingEnviroment;
        public UserController(
            IMapper _mapper, 
            IUsuarioService userService,
            IPersonaService personService,
            IWebHostEnvironment hostingEnviroment)
        {
            mapper = _mapper;
            this.userService = userService;
            this.personService = personService;
            this.hostingEnviroment = hostingEnviroment;
        }

        /// <summary>
        /// Método para registrar un usuario
        /// </summary>
        /// <remarks>
        /// En caso de haber una foto se procede a guardarlo en una carpeta del servidor para
        /// obtener la url de la imagen y añadirlo a la propiedad Foto.
        /// </remarks>
        /// <param name="usuarioDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Save(UsuarioDTO usuarioDTO)
        {
            var oUsuario = mapper.Map<Usuario>(usuarioDTO);
            oUsuario.Persona.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(usuarioDTO.ImageBase64,false);
            userService.InsertUserPerson(oUsuario);
            return Ok(new { Message = "Usuario registrado con éxito! Revise su correo y confirme su cuenta para poder acceder" });
        }

        /// <summary>
        /// Método para actualizar la información personal de un usuario
        /// </summary>
        /// <param name="personaDTO"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePerson/{id}")]
        public IActionResult UpdatePerson(PersonaDTO personaDTO, long id)
        {
            if (personaDTO.Id != id || personaDTO.Id==0)
                return BadRequest("El código del usuario no es válido");
            var oPersona = mapper.Map<Persona>(personaDTO);
            personService.UpdatePerson(oPersona);
            return Ok(new { Message = "Datos personales actualizados!" });
        }

        /// <summary>
        /// Método para obtener la información personal de un usuario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(long id)
        {
            var oPersona = personService.GetById(id);
            var personaDTO = mapper.Map<PersonaDTO>(oPersona);
            return Ok(personaDTO);
        }

        /// <summary>
        /// Método para actualizar la foto de un usuario
        /// </summary>
        /// <remarks>
        /// Se obtiene la ruta de la imagen anterior del usuario para eliminarlo y se 
        /// la url de la imagen actual para añadirlo a la propiedad Foto
        /// </remarks>
        /// <param name="filePhotoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePhoto")]
        public IActionResult UpdatePhoto(FilePhotoViewModel filePhotoViewModel)
        {
            if (filePhotoViewModel.Id == 0)
                return BadRequest("El código del usuario no es válido");
            long id = Convert.ToInt64(filePhotoViewModel.Id);
            filePhotoViewModel.Photo = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(filePhotoViewModel.Photo,false);
            FileHelper.RemoveImage(personService.GetPathPhoto(id));
            personService.UpdatePhoto(filePhotoViewModel);
            return Ok(new { Message = "Foto actualizada con éxito!" });
        }

        /// <summary>
        /// Método que verifica si el email ya existe
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckEmail/{email}")]
        public IActionResult CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("El email del usuario no es válido");
            bool available = !userService.CheckEmail(email);
            return Ok(available);
        }

        /// <summary>
        /// Método que verifica si el teléfono ya existe
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckPhone/{phone}")]
        public IActionResult CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return BadRequest("El teléfono del usuario no es válido");
            bool available = !personService.CheckPhone(phone);
            return Ok(available);
        }

        /// <summary>
        /// Método que actualiza el email del usuario
        /// </summary>
        /// <remarks>
        /// Se realiza una verificación previa de las credenciales actuales del usuario 
        /// para proceder a editar el email
        /// </remarks>
        /// <param name="userEmailViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdateEmail")]
        public IActionResult UpdateEmail(UserEmailViewModel userEmailViewModel)
        {
            if (userEmailViewModel.Id == 0 || userEmailViewModel.Id == null)
                return BadRequest("El código del usuario no es válido");
            if (string.IsNullOrEmpty(userEmailViewModel.ClaveActual)) 
                return BadRequest("La clave actual es requerida");
            var userPass = new UserPasswordViewModel()
            {
                Clave = userEmailViewModel.ClaveActual,
                Id = userEmailViewModel.Id 
            };
            if (!userService.CheckPassword(userPass))
                return BadRequest("La clave actual no es válida");
            userService.UpdateEmail(userEmailViewModel);
            return Ok(new { Message = "Email actualizado con éxito! Revise su correo y confirme su cuenta para poder acceder" });
        }

        /// <summary>
        /// Método para actualizar la clave actual del usuario
        /// </summary>
        /// <remarks>
        /// Se realiza una verificación previa de las credenciales actuales del usuario 
        /// para proceder a editar la contraseña
        /// </remarks>
        /// <param name="userPassViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePassword")]
        public IActionResult UpdateClave(UserPasswordViewModel userPassViewModel)
        {
            if (userPassViewModel.Id == 0 || userPassViewModel.Id == null)
                return BadRequest("El código del usuario no es válido");
            if (userService.CheckPassword(userPassViewModel))
                return BadRequest("La clave no es válida");
            userService.UpdateClave(userPassViewModel);
            return Ok(new { Message = "Clave actualizada con éxito!" });
        }

        /// <summary>
        /// Método para obtener la ruta de la foto de un usuario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetPathPhoto/{id}")]
        public IActionResult GetPathPhoto(long? id)
        {
            if (id == 0 || id==null)
                return BadRequest("El código del usuario no es válido");
            var path = personService.GetPathPhoto(id.Value);
            return Ok(path);
        }

        /// <summary>
        /// Método para obtener la fecha actual del servidor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCurrentDate")]
        public IActionResult GetCurrentDate()
        {
            var currentDate = DateHelper.GetCurrentDate();
            return Ok(currentDate);
        }

        /// <summary>
        /// Método para recuperar el acceso a la cuenta
        /// </summary>
        /// <remarks>
        /// Se verifica primero si el email del usuario y el estado de la cuenta es válido.
        /// Posteriormente se envía un correo de recuperación expirable al usuario
        /// </remarks>
        /// <param name="userEmailViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RecoveryAccess")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult RecoveryAccess(UserEmailViewModel userEmailViewModel)
        {
            if (userEmailViewModel.Id == 0 || userEmailViewModel.Id == null)
                return BadRequest("El código del usuario no es válido");
            if (!userService.CheckEmail(userEmailViewModel.Email))
                return BadRequest("El email del usuario no es válido");
            if (!userService.VerifyStatusUser(userEmailViewModel.Id.Value))
                return BadRequest("Su cuenta no está confirmada o está inactiva");

            userService.RecoveryAccess(userEmailViewModel, DateHelper.GetCurrentDate());
            return Ok(new { Message = "Enlace generado revise su correo!" });
        }

        /// <summary>
        /// Método para verificar que el token de autenticación sea válido
        /// </summary>
        /// <param name="tokenValidViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckToken")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult CheckToken(TokenValidViewModel tokenValidViewModel)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var checkToken = userService.CheckToken(tokenValidViewModel,currentDate);
            return Ok(checkToken);
        }

        /// <summary>
        /// Método para realizar el cambio de la clave de un usuario que solicitó la recuperación del acceso a su cuenta
        /// </summary>
        /// <remarks>
        /// Después de que el usuario accede al enlace de recuperación enviado a su correo,
        /// se íngresa la nueva contraseña y se envían los datos. Antes de realizar el cambio
        /// se verifica la validez del token de autenticación
        /// </remarks>
        /// <param name="tokenPassViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult ChangePassword(TokenPasswordViewModel tokenPassViewModel)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var tokenValidViewModel = new TokenValidViewModel() { Token = tokenPassViewModel.Token };
            if (!userService.CheckToken(tokenValidViewModel, currentDate))
                return BadRequest("Enlace de recuperación caducado");
            if (userService.ChangeClave(tokenPassViewModel))
                return Ok(new { Message = "Clave cambiada con éxito!" });
            return BadRequest("Operación no realizada token inválido");
        }

        /// <summary>
        /// Método para confirmar el correo del usuario
        /// </summary>
        /// <remarks>
        /// Se verifica la validez del token de autenticación y el estado sin-confirmar(3) de la cuenta.
        /// Después se actualiza el token y el estado de la cuenta. Para que el usuario pueda acceder 
        /// posteriormente a su cuenta
        /// </remarks>
        /// <param name="tokenValidViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ConfirmEmail")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult ConfirmEmail(TokenValidViewModel tokenValidViewModel)
        {
            bool confirm = userService.ConfirmEmail(tokenValidViewModel);
            return Ok(confirm);
        }
    }
}
