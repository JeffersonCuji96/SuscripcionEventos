using Api.Filters;
using AutoMapper;
using BL.DTO;
using BL.Models;
using BL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUsuarioService userService;

        public AccessController(IMapper _mapper, IUsuarioService userService)
        {
            mapper = _mapper;
            this.userService = userService;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public ActionResult Login(AccesoDTO accesoDTO)
        {
            var oUsuario = mapper.Map<Usuario>(accesoDTO);
            var oAccessViewModel = userService.Login(oUsuario);
            if (oAccessViewModel.IdUsuario == 0)
                return StatusCode(StatusCodes.Status403Forbidden, "Usuario o Clave incorrecta");
            return Ok(oAccessViewModel);
        }
    }
}
