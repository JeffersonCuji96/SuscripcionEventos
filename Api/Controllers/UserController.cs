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
    }
}
