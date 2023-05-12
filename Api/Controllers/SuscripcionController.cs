using Api.Filters;
using AutoMapper;
using BL.DTO;
using BL.Models;
using BL.Services;
using BL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuscripcionController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ISuscripcionService suscriptionService;
        private readonly IEventoService eventService;
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<NotificationViewModel> _notificationCollection;

        public SuscripcionController(
           IMapper _mapper,
           ISuscripcionService suscriptionService,
           IEventoService eventService,
           IMongoClient mongoClient)
        {
            mapper = _mapper;
            this.suscriptionService = suscriptionService;
            this.eventService = eventService;
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("suscriptiondb");
            _notificationCollection = database.GetCollection<NotificationViewModel>("notification");
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
            if (suscribe.IdUsuario == 0)
                return BadRequest("El código del usuario no es válido");
            if (suscribe.IdEvento == 0)
                return BadRequest("El código del evento no es válido");
            return Ok(suscriptionService.CheckSuscribeUser(suscribe));
        }

        [HttpGet]
        [Route("GetByUserSuscriptions/{id}")]
        public IActionResult GetByUserSuscriptions(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            var events = suscriptionService.GetSuscriptionsByUser(id);
            return Ok(events);
        }

        [HttpGet]
        [Route("GetEventsTodayByUser/{id}")]
        public IActionResult GetEventsTodayByUser(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            var idsEvents = eventService.GetEventsTodayByUser(id);
            return Ok(idsEvents);
        }

        [HttpGet]
        [Route("GetNotificationsMongoDb/{id}")]
        public async Task<IActionResult> GetNotificationsMongoDb(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            
            var filtro = Builders<NotificationViewModel>.Filter.And(
                Builders<NotificationViewModel>.Filter.Eq("idusuariosuscrito", id),
                Builders<NotificationViewModel>.Filter.Ne("estado", 3));

            var opciones = new FindOptions<NotificationViewModel>
            {
                Sort = Builders<NotificationViewModel>.Sort.Descending("_id"),
                Limit = 5
            };

            if (filtro ==null && opciones == null)
                return BadRequest("Error parámetros inválidos");
            var lstNotifications = await _notificationCollection.FindAsync(filtro,opciones);
            var results = await lstNotifications.ToListAsync();
            return Ok(results);
        }

        [HttpDelete]
        [Route("RemoveNotification/{id}")]
        public async Task<IActionResult> RemoveNotification(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Código de notificación inválido");
            var filter = Builders<NotificationViewModel>.Filter.Eq("Id",id); 
            var update = Builders<NotificationViewModel>.Update.Set("Estado",3);
            var updateResult = await _notificationCollection.UpdateOneAsync(filter, update);
            return Ok(updateResult.ModifiedCount);
        }

        [HttpPost]
        [Route("GetNotificationEvent")]
        public async Task<IActionResult> GetNotificationEvent(EventNotificationViewModel notificationEvent)
        {
            if (notificationEvent.IdEvento == 0)
                return BadRequest("Código de evento inválido");
            if (string.IsNullOrEmpty(notificationEvent.IdNotificacion))
                return BadRequest("Código de notificación inválido");

            if (notificationEvent.Estado == 1)
            {
                var filter = Builders<NotificationViewModel>.Filter.Eq("Id", notificationEvent.IdNotificacion);
                var update = Builders<NotificationViewModel>.Update.Set("Estado", 2);
                await _notificationCollection.UpdateOneAsync(filter, update);
            }

            var eventNotification = eventService.GetEventNotification(notificationEvent.IdEvento);
            if (eventNotification == null)
                return BadRequest("El evento no existe o ha sido eliminado");

            return Ok(eventNotification);
        }

    }
}
