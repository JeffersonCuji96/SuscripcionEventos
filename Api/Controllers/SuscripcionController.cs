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

        /// <summary>
        /// Método para realizar la suscripción de un usuario a un evento
        /// </summary>
        /// <remarks>
        /// Se obtiene el evento con los datos específicos que se necesitan para validar
        /// la suscripción, del evento al que se va a suscribir un usuario
        /// </remarks>
        /// <param name="suscripcionDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult Suscribe(SuscripcionDTO suscripcionDTO)
        {
            long idSuscripcion;
            var oSuscripcion = mapper.Map<Suscripcion>(suscripcionDTO);
            var eventViewModel = eventService.GetDataEventCheck(oSuscripcion.IdEvento);
            eventViewModel.IdUsuario = oSuscripcion.IdUsuario;

            var strCheckDateEventSuscription = suscriptionService.CheckDateEventSuscription(eventViewModel);
            if (!string.IsNullOrEmpty(strCheckDateEventSuscription))
                return BadRequest(strCheckDateEventSuscription);

            if (suscriptionService.CheckSuscribeUser(new SuscribeCheckViewModel { 
                IdEvento = suscripcionDTO.IdEvento, IdUsuario= suscripcionDTO.IdUsuario }).Item1 == 0)
                idSuscripcion = suscriptionService.Insert(oSuscripcion).Id;
            else idSuscripcion = suscriptionService.SuscribeEvent(oSuscripcion.Id, true); 
            return Ok(new { Message = "Se ha suscrito con éxito!", Id =idSuscripcion });
        }

        /// <summary>
        /// Método para desuscribir al usuario de un evento
        /// </summary>
        /// <param name="suscripcionDTO"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Unsuscribe(SuscripcionDTO suscripcionDTO, long id)
        {
            if (suscripcionDTO.Id != id || suscripcionDTO.Id == 0)
                return BadRequest("El código de suscripción no es válido");

            suscriptionService.SuscribeEvent(id, false);
            return Ok(new { Message = "Se ha desuscrito con éxito!" });
        }

        /// <summary>
        /// Método para verificar si un usuario está suscrito a un evento
        /// </summary>
        /// <param name="suscribe"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método par obtener un listado de las suscripciones de un usuario
        /// </summary>
        /// <remarks>
        /// Junto a las suscripciones se agrega la cantidad de suscriptores que tiene cada evento 
        /// al que esté suscrito el usuario para posteriormente usar esa información en el detalle
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetByUserSuscriptions/{id}")]
        public IActionResult GetByUserSuscriptions(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            var events = suscriptionService.GetSuscriptionsByUser(id);
            return Ok(events);
        }

        /// <summary>
        /// Método para obtener los identificadores de cada evento al que está suscrito el usuario
        /// </summary>
        /// <remarks>
        /// Solo se obtiene la información que sea de la fecha actual, para notificar cambios de 
        /// los eventos en los que está suscrito el usuario
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSuscriptionsTodayByUser/{id}")]
        public IActionResult GetSuscriptionsTodayByUser(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            var idsEvents = eventService.GetSuscriptionsTodayByUser(id);
            return Ok(idsEvents);
        }

        /// <summary>
        /// Método para obtener los identificadores de cada evento que ha organizado el usuario
        /// </summary>
        /// <remarks>
        /// Solo se obtiene la información que sea de la fecha actual, para notificar cambios en 
        /// los eventos que el usuario ha organizado
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEventsTodayByUser/{id}")]
        public IActionResult GetEventsTodayByUser(long id)
        {
            if (id == 0)
                return BadRequest("El código del usuario no es válido");
            var idsEvents = eventService.GetEventsTodayByUser(id);
            return Ok(idsEvents);
        }

        /// <summary>
        /// Método para obtener las notificaciones de una base de datos no relacional
        /// </summary>
        /// <remarks>
        /// Se filtra las notificaciones por un usuario especifico y de un estado que no 
        /// sea eliminado(3). Además se agrega un opción para retornar solamente las 10 
        /// últimas notificaciones
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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
                Limit = 10
            };

            if (filtro ==null && opciones == null)
                return BadRequest("Error parámetros inválidos");
            var lstNotifications = await _notificationCollection.FindAsync(filtro,opciones);
            var results = await lstNotifications.ToListAsync();
            return Ok(results);
        }

        /// <summary>
        /// Método para realizar la eliminación lógica de una notificación
        /// </summary>
        /// <remarks>
        /// Se agrega un filtro para una notificación especifica y una opción
        /// para el cambio de estado a eliminado(3).
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Método para obtener los datos de una notificación y cambiar el estado a visualizado
        /// </summary>
        /// <remarks>
        /// Se verifica si el estado actual de la notificación es activo(1) para proceder a cambiarlo a un estado
        /// de visualizado(2). Posteriormente se obtiene los datos del evento que representa la notificación
        /// </remarks>
        /// <param name="notificationEvent"></param>
        /// <returns></returns>
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
                return BadRequest("El detalle del evento no se puede obtener porque ha sido eliminado");

            return Ok(eventNotification);
        }
    }
}
