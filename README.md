# Sistema de suscripción a eventos y notificaciones
El sistema muestra los eventos disponibles para que el usuario se suscriba, cuando lo hace ingresa a un grupo de notificaciones 
que pertenece a ese evento y como recordatorio se le notifica una hora antes de que inicie. Las notificaciones se envían en tiempo 
real a los usuarios conectados, y a los que no, al momento de que inicien sesión podrán revisarlas ya que estas se guardan, para 
mantener un historial y realizar cambios en ella cuando el usuario vea o elimine alguna notificación. Además la aplicación refleja 
los cambios de estado de los eventos en tiempo real, cuando pasan de estar activos a notificados o procesados. El proyecto se desarrolló 
en angular y .NET 6 Web API que usa el patrón de repositorio, swagger para documentar la API y base de datos SqlServer. Para las 
notificaciones se utilizó SignalR junto a un servicio de fondo para la ejecución de tareas en segundo plano y MongoDB para guardar 
el historial de las notificaciones.

## Demostración

Para revisar la demo del proyecto debe ingresar al sitio web: https://suscripcion-eventos.web.app

Para ver un tutorial del proyecto visitar en youtube: https://youtu.be/IFV9jFgH-5M

    • Para iniciar sesión primero debe registrarse con su correo y confirmar su cuenta
    • La documentación de la api: https://suscripcion-eventos.azurewebsites.net/swagger
    
## Vista previa

Inicio

![](https://github.com/JeffersonCuji96/SuscripcionEventos/blob/master/vista-previa.png)

## Diagrama entidad relación

![](https://github.com/JeffersonCuji96/SuscripcionEventos/blob/master/diagrama-entidad-relacion.png)

## Diagrama de flujo principal

![](https://github.com/JeffersonCuji96/SuscripcionEventos/blob/master/diagrama-flujo.png)
