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
    public class EventoController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IEventoService eventService;
        private readonly ICategoriaService categoriaService;
        private readonly IWebHostEnvironment hostingEnviroment;

        public EventoController(
           IMapper _mapper,
           IEventoService eventService,
           ICategoriaService categoriaService,
           IWebHostEnvironment hostingEnviroment)
        {
            mapper = _mapper;
            this.eventService = eventService;
            this.categoriaService = categoriaService;
            this.hostingEnviroment = hostingEnviroment;
        }

        [HttpGet]
        [Route("GetEventosSuscripciones/{id?}")]
        public IActionResult Get(int id = 0)
        {
            var events = eventService.GetEventsSuscriptions(id);
            return Ok(events);
        }

        [HttpGet]
        [Route("GetByUserEventos/{id}")]
        public IActionResult GetByUserEventos(long id)
        {
            var events = eventService.GetEventsByUser(id);
            var eventsDTO = events.Select(x => mapper.Map<EventoDTO>(x));
            return Ok(eventsDTO);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Save(EventoDTO eventoDTO)
        {
            var strCheckDate = eventService.CheckDateEvent(eventoDTO, 0);
            if (!string.IsNullOrEmpty(strCheckDate))
                return BadRequest(strCheckDate);
            
            var oEvento = mapper.Map<Evento>(eventoDTO);
            oEvento.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(eventoDTO.ImageBase64,true);
            eventService.Insert(oEvento);
            return Ok(new { Message = "Evento registrado con éxito!" });
        }

        [HttpGet]
        [Route("GetCategorias")]
        public IActionResult GetCategorias()
        {
            var categorias = categoriaService.GetAll();
            var categoriasDTO = categorias.Select(x => mapper.Map<CategoriaDTO>(x));
            return Ok(categoriasDTO);
        }

        [HttpDelete]
        [Route("RemoveEvent/{id}")]
        public IActionResult RemoveEvent(long id)
        {
            eventService.RemoveEvent(id);
            return Ok(new { Message = "Evento eliminado con éxito!" });
        }

        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [Route("UpdateEvent/{id}/{checkImage}")]
        public IActionResult UpdateEvent(EventoDTO eventoDTO, long id, bool checkImage)
        {
            if (eventoDTO.Id != id || eventoDTO.Id == 0)
                return BadRequest("El código del evento no es válido");
            
            var strCheckDate = eventService.CheckDateEvent(eventoDTO, id);
            if (!string.IsNullOrEmpty(strCheckDate))
                return BadRequest(strCheckDate);

            var oEvento = mapper.Map<Evento>(eventoDTO);
            if (checkImage)
            {
                oEvento.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(eventoDTO.ImageBase64, true);
                FileHelper.RemoveImage(eventService.GetPathPhoto(id));
            }
            eventService.UpdateEvent(oEvento, checkImage);
            return Ok(new { Message = "Evento actualizado!" });
        }
    }
}
