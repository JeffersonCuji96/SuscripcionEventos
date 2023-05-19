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

        /// <summary>
        /// Método que realiza la autenticación del usuario
        /// </summary>
        /// <remarks>
        /// La verificación puede obtener tres posibles estados, el primero en caso de que las credenciales sean 
        /// incorrectas, el segundo si la cuenta del usuario esta inactiva y el tercero si la cuenta aún no ha 
        /// sido confirmado por el correo del usuario
        /// </remarks>
        /// <param name="accesoDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public ActionResult Login(AccesoDTO accesoDTO)
        {
            var oUsuario = mapper.Map<Usuario>(accesoDTO);
            var oAccessViewModel = userService.Login(oUsuario);

            switch (oAccessViewModel.Item2)
            {
                case 0:
                    return StatusCode(403, "Usuario o Clave incorrecta");
                case 2:
                    return StatusCode(202, "Cuenta inactiva");
                case 3:
                    return StatusCode(201,"Cuenta no confirmada");
                default:
                    break;
            }
            return Ok(oAccessViewModel);
        }
    }
}
