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
        private readonly IWebHostEnvironment hostingEnviroment;

        public EventoController(
           IMapper _mapper,
           IEventoService eventService,
           IWebHostEnvironment hostingEnviroment)
        {
            mapper = _mapper;
            this.eventService = eventService;
            this.hostingEnviroment = hostingEnviroment;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Save(EventoDTO eventoDTO)
        {
            var oEvento = mapper.Map<Evento>(eventoDTO);
            if (!string.IsNullOrEmpty(eventoDTO.ImageBase64))
                oEvento.Foto = hostingEnviroment.ContentRootPath + FileHelper.UploadImage(eventoDTO.ImageBase64);
            eventService.Insert(oEvento);
            return Ok(new { Message = "Evento registrado con éxito!" });
        }
    }
}
