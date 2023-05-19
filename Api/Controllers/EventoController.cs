using Api.Filters;
using Api.Helpers;
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

        /// <summary>
        /// Método para obtener los eventos con la cantidad de suscriptores
        /// </summary>
        /// <remarks>
        /// Se recibe la categoría como parámetro, en caso de ser 0 se retorna todos el listado de 
        /// manera predeterminada caso contrario se filtran los datos por categoria
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetEventosSuscripciones/{id?}")]
        public IActionResult Get(int id = 0)
        {
            var events = eventService.GetEventsSuscriptions(id);
            return Ok(events);
        }

        /// <summary>
        /// Método para obtener los eventos organizados por el usuario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetByUserEventos/{id}")]
        public IActionResult GetByUserEventos(long id)
        {
            var events = eventService.GetEventsByUser(id);
            var eventsDTO = events.Select(x => mapper.Map<EventoDTO>(x));
            return Ok(eventsDTO);
        }

        /// <summary>
        /// Método para crear un evento 
        /// </summary>
        /// <remarks>
        /// Antes de realizar el registro se verifica si las fechas de inicio y/o fin del evento 
        /// coinciden con las fechas de otros eventos creados por el mismo usuario. Devuelve un 
        /// mensaje si hay alguna coincidencia y una cadena vacía si no hay coincidencias.
        /// En caso de haber una foto se procede a guardarlo en una carpeta del servidor para
        /// obtener la url de la imagen y añadirlo a la propiedad Foto.
        /// </remarks>
        /// <param name="eventoDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
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

        /// <summary>
        /// Método para obtener las categorías del evento
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCategorias")]
        public IActionResult GetCategorias()
        {
            var categorias = categoriaService.GetAll();
            var categoriasDTO = categorias.Select(x => mapper.Map<CategoriaDTO>(x));
            return Ok(categoriasDTO);
        }

        /// <summary>
        /// Método para realizar la eliminación lógica de un evento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [Route("RemoveEvent/{id}")]
        public IActionResult RemoveEvent(long id)
        {
            eventService.RemoveEvent(id);
            return Ok(new { Message = "Evento eliminado con éxito!" });
        }

        /// <summary>
        /// Método para actualizar la información de un evento
        /// </summary>
        /// <remarks>
        /// Antes de realizar la actualización se verifica si las fechas de inicio y/o fin del evento 
        /// coinciden con las fechas de otros eventos creados por el mismo usuario. Devuelve un 
        /// mensaje si hay alguna coincidencia y una cadena vacía si no hay coincidencias.
        /// En caso de haber una foto para actualizar se procede a guardarlo en una carpeta del servidor 
        /// para obtener la url de la imagen y modificar a la propiedad Foto. Después se obtiene la url de la
        /// imagen anterior mediante el identificador del evento para proceder a eliminarlo de la carpeta del servidor.
        /// </remarks>
        /// <param name="eventoDTO"></param>
        /// <param name="id"></param>
        /// <param name="checkImage"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
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
