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
        private readonly IEventoService eventService;

        public SuscripcionController(
           IMapper _mapper,
           ISuscripcionService suscriptionService,
           IEventoService eventService)
        {
            mapper = _mapper;
            this.suscriptionService = suscriptionService;
            this.eventService = eventService;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Suscribe(SuscripcionDTO suscripcionDTO)
        {
            long idSuscripcion;
            var oSuscripcion = mapper.Map<Suscripcion>(suscripcionDTO);
            var eventViewModel = eventService.GetDataEventCheck(oSuscripcion.IdEvento);
            eventViewModel.IdUsuario = oSuscripcion.IdUsuario;

            var strCheckDateEvent = eventService.CheckDateEvent(new EventoDTO { 
                FechaInicio=eventViewModel.FechaInicio, FechaFin=eventViewModel.FechaFin,
                IdUsuario = eventViewModel.IdUsuario
            },0);
            if (!string.IsNullOrEmpty(strCheckDateEvent))
                return BadRequest($"No se puede suscribir debido a que {strCheckDateEvent.ToLower()}");

            var strCheckDateEventSuscription = suscriptionService.CheckDateEventSuscription(eventViewModel);
            if (!string.IsNullOrEmpty(strCheckDateEventSuscription))
                return BadRequest(strCheckDateEventSuscription);

            if (suscriptionService.CheckSuscribeUser(new SuscribeCheckViewModel { 
                IdEvento = suscripcionDTO.IdEvento, IdUsuario= suscripcionDTO.IdUsuario }).Item1 == 0)
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
