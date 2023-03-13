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

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Save(UsuarioDTO usuarioDTO)
        {
            var oUsuario = mapper.Map<Usuario>(usuarioDTO);
            oUsuario.Persona.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(usuarioDTO.ImageBase64);
            userService.InsertUserPerson(oUsuario);
            return Ok(new { Message = "Usuario registrado con éxito! Revise su correo y confirme su cuenta para poder acceder" });
        }

        [HttpPut]
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

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var oPersona = personService.GetById(id);
            var personaDTO = mapper.Map<PersonaDTO>(oPersona);
            return Ok(personaDTO);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePhoto")]
        public IActionResult UpdatePhoto(FilePhotoViewModel filePhotoViewModel)
        {
            if (filePhotoViewModel.Id == 0)
                return BadRequest("El código del usuario no es válido");
            long id = Convert.ToInt64(filePhotoViewModel.Id);
            filePhotoViewModel.Photo = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(filePhotoViewModel.Photo);
            FileHelper.RemoveImage(personService.GetPathPhoto(id));
            personService.UpdatePhoto(filePhotoViewModel);
            return Ok(new { Message = "Foto actualizada con éxito!" });
        }

        [HttpGet]
        [Route("CheckEmail/{email}")]
        public IActionResult CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("El email del usuario no es válido");
            bool available = !userService.CheckEmail(email);
            return Ok(available);
        }

        [HttpGet]
        [Route("CheckPhone/{phone}")]
        public IActionResult CheckPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return BadRequest("El teléfono del usuario no es válido");
            bool available = !personService.CheckPhone(phone);
            return Ok(available);
        }

        [HttpPost]
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

        [HttpPost]
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

        [HttpGet]
        [Route("GetPathPhoto/{id}")]
        public IActionResult GetPathPhoto(long? id)
        {
            if (id == 0 || id==null)
                return BadRequest("El código del usuario no es válido");
            var path = personService.GetPathPhoto(id.Value);
            return Ok(path);
        }

        [HttpGet]
        [Route("GetCurrentDate")]
        public IActionResult GetCurrentDate()
        {
            var currentDate = DateHelper.GetCurrentDate();
            return Ok(currentDate);
        }

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

        [HttpPost]
        [Route("CheckToken")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult CheckToken(TokenValidViewModel tokenValidViewModel)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var checkToken = userService.CheckToken(tokenValidViewModel,currentDate);
            return Ok(checkToken);
        }

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
