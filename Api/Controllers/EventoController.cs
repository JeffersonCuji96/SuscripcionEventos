﻿using Api.Filters;
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

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Save(EventoDTO eventoDTO)
        {
            var strCheckDate = eventService.CheckDateEvent(eventoDTO.FechaInicio, eventoDTO.FechaFin, eventoDTO.IdUsuario);
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
    }
}
