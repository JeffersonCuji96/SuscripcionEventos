using Api.Filters;
using Api.Helpers;
using AutoMapper;
using BL.DTO;
using BL.Models;
using BL.Services;
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
        private readonly IWebHostEnvironment hostingEnviroment;
        public UserController(
            IMapper _mapper, 
            IUsuarioService userService,
            IWebHostEnvironment hostingEnviroment)
        {
            mapper = _mapper;
            this.userService = userService;
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
    }
}
