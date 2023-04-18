using Api.Filters;
using AutoMapper;
using BL.DTO;
using BL.Models;
using BL.Services;
using BL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscripcionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISuscripcionService suscriptionService;

        public SuscripcionController(
           IMapper _mapper,
           ISuscripcionService suscriptionService)
        {
            mapper = _mapper;
            this.suscriptionService = suscriptionService;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Suscribe(SuscripcionDTO suscripcionDTO)
        {
            long idSuscripcion = 0;
            var oSuscripcion = mapper.Map<Suscripcion>(suscripcionDTO);
            var suscribeCheck = new SuscribeCheckViewModel()
            {
                IdEvento=suscripcionDTO.IdEvento,
                IdUsuario=suscripcionDTO.IdUsuario
            };
            if (suscriptionService.CheckSuscribeUser(suscribeCheck).Item1 == 0)
                idSuscripcion = suscriptionService.Insert(oSuscripcion).Id;
            else idSuscripcion = suscriptionService.SuscribeEvent(oSuscripcion.Id, true); 
            return Ok(new { Message = "Se ha suscrito con éxito!", Id =idSuscripcion });
        }

        [HttpPut("{id}")]
        public IActionResult Unsuscribe(SuscripcionDTO suscripcionDTO, long id)
        {
            if (suscripcionDTO.Id != id || suscripcionDTO.Id == 0)
                return BadRequest("El código de suscripción no es válido");

            suscriptionService.SuscribeEvent(id, false);
            return Ok(new { Message = "Se ha desuscrito con éxito!" });
        }

        [HttpPost]
        [Route("CheckSuscribe")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult CheckSuscribeUser(SuscribeCheckViewModel suscribe)
        {
            return Ok(suscriptionService.CheckSuscribeUser(suscribe));
        }
    }
}
