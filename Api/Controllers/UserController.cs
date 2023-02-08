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
            if (!string.IsNullOrEmpty(usuarioDTO.ImageBase64))
                oUsuario.Persona.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(usuarioDTO.ImageBase64);
            userService.InsertUserPerson(oUsuario);
            return Ok(new { Message = "Usuario registrado con éxito!" });
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
            var oUsuario = userService.GetUserPersonById(id);
            var usuarioDTO = mapper.Map<UsuarioDTO>(oUsuario);
            if (usuarioDTO == null)
                return NotFound();
            return Ok(usuarioDTO);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePhoto")]
        public IActionResult UpdatePhoto(FilePhotoViewModel fileData)
        {
            if (fileData.Id == 0)
                return BadRequest("El código del usuario no es válido");
            long id = Convert.ToInt64(fileData.Id);
            string foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(fileData.Photo);
            FileHelper.RemoveImage(personService.GetPathPhoto(id));
            personService.UpdatePhoto(foto, id);
            return Ok(new { Message = "Foto actualizada con éxito!" });
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("CheckPassword")]
        public IActionResult CheckPassword(UserPasswordViewModel data)
        {
            if(data.Id==0 || data.Id==null)
                return BadRequest("El código del usuario no es válido");
            return Ok(userService.CheckPassword(data.Clave, data.Id.Value));
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
        public IActionResult UpdateEmail(UserEmailViewModel data)
        {
            if (data.Id == 0 || data.Id == null)
                return BadRequest("El código del usuario no es válido");
            userService.UpdateEmail(data.Email, data.Id.Value);
            return Ok(new { Message = "Email actualizado con éxito!" });
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdatePassword")]
        public IActionResult UpdateClave(UserPasswordViewModel data)
        {
            if (data.Id == 0 || data.Id == null)
                return BadRequest("El código del usuario no es válido");
            userService.UpdateClave(data.Clave, data.Id.Value);
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
            var currentDate = DateHelper.GetCurrentDate().Date;
            return Ok(currentDate);
        }

        [HttpPost]
        [Route("RecoveryAccess")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult RecoveryAccess(UserEmailViewModel data)
        {
            if (data.Id == 0 || data.Id == null)
                return BadRequest("El código del usuario no es válido");
            if (!userService.CheckEmail(data.Email))
                return BadRequest("El email del usuario no es válido");

            userService.RecoveryAccess(data, DateHelper.GetCurrentDate());
            return Ok(new { Message = "Enlace generado revise su correo!" });
        }

        [HttpPost]
        [Route("CheckToken")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult CheckToken(TokenValidViewModel data)
        {
            var currentDate = DateHelper.GetCurrentDate();
            var checkToken = userService.CheckToken(currentDate,data.Token);
            return Ok(checkToken);
        }
    }
}
