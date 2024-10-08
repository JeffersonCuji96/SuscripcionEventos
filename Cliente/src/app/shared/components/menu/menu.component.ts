import { NgZone, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';
import { HubService } from 'src/app/app-services/hub.service';
import { MessageViewModel } from 'src/app/core/models/view-models/messageViewModel';
import { NotificationViewModel } from 'src/app/core/models/view-models/notificationViewModel';
import { Helpers } from 'src/app/helpers/helper';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { EventoService } from 'src/app/modules/evento/services/evento.service';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  @ViewChild('menuDesplegable') menuDesplegable!: ElementRef;
  @ViewChild('toast') toast: ElementRef;
  @ViewChild('bell') bell: ElementRef;
  @ViewChild('notifDropdown') notifDropdown: ElementRef;
  @ViewChild('linkDropdown') linkDropdown: ElementRef;

  public fullName: string;
  public message: MessageViewModel = null;
  public notifications: NotificationViewModel;
  private connectionHub: HubConnection;
  private stop$ = new Subject<void>();
  public maxWidth = 992;

  constructor(
    private authService: AuthService,
    private hubService: HubService,
    private eventService: EventoService,
    private ngZone: NgZone,
    private suscriptionService: SuscripcionService,
    private helper: Helpers,
    private router: Router) {
    this.fullName = authService.getFullName();
    this.connectionHub = this.hubService.getConnectionHub();
  }

  ngOnInit(): void {
    this.notifiedJoinSuscriptionChange();
    this.notifiedJoinEventChange();
    this.startHubNotification();
    this.getNotifications();
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  startHubNotification() {
    let id = this.authService.getIdUserLocalStorage();
    this.connectionHub.start().then(_ => {
      this.joinSuscriptionsGroup();
      this.joinEventsGroup();
      this.connectionHub.on("Notification", msg => {
        this.suscriptionService.suscriptionChange.emit(true);
        this.eventService.checkEventOrganizer({
          IdEvento:Number(msg.Grupo),
          IdUsuario:id
        }).pipe(
          takeUntil(this.stop$))
          .subscribe(res => {
            if (res === false) {
              this.showNotification(msg);
              this.shakeBells();
              this.getNotifications();
            }
          });
      });
      this.connectionHub.on("Notified", evt => this.eventService.notifiedEventChange.emit(evt));
      this.connectionHub.on("Processed", evt => this.eventService.processedEventChange.emit(evt));
    });
  }

  logout() {
    this.closeToggler();
    this.authService.removeSesion();
    window.location.href = "/auth/login";
  }

  getNotifications() {
    let id = this.authService.getIdUserLocalStorage();
    this.suscriptionService.getNotificationsMongoDb(id).pipe(
      takeUntil(this.stop$))
      .subscribe((res: any) => {
        this.notifications = res;
        res.length === 0 ? this.hideMenuNotif() : null;
      },
        error => {
          this.helper.manageErrors(error);
        });
  }

  notifiedJoinSuscriptionChange() {
    this.suscriptionService.notifiedSuscriptionChange.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        res ? this.joinSuscriptionsGroup() : null;
      });
  }

  notifiedJoinEventChange() {
    this.eventService.notifiedJoinEvent.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        res ? this.joinEventsGroup() : null;
      });
  }

  removeNotification(idNotif: any) {
    this.suscriptionService.removeNotification(idNotif)
      .pipe(takeUntil(this.stop$))
      .subscribe(res => {
        if (res === 1) {
          this.getNotifications();
          this.showMenuNotif();
        }
      },
        error => {
          this.helper.manageErrors(error);
        });
  }

  showMenuNotif() {
    this.linkDropdown.nativeElement.classList.add("show");
    this.notifDropdown.nativeElement.classList.add("show");
  }

  hideMenuNotif() {
    this.linkDropdown.nativeElement.classList.remove("show");
    this.notifDropdown.nativeElement.classList.remove("show");
  }

  showPageDetailEvent(notf: any) {
    this.closeToggler();
    this.suscriptionService.getNotificationEvent({
      IdEvento: notf.IdEvento,
      IdNotificacion: notf.Id,
      Estado: notf.Estado
    }).pipe(takeUntil(this.stop$))
      .subscribe(event => {
        event.Limit = 0;
        this.getNotifications();
        this.router.navigate(['/evento/detail', btoa(JSON.stringify(event))], { skipLocationChange: true });
      },
        error => {
          this.helper.manageErrors(error);
        });
  }

  closeToggler(): void {
    const windowWidth = window.innerWidth;
    if (windowWidth < this.maxWidth) {
      this.menuDesplegable.nativeElement.click();
    }
  }

  showNotification(msg: MessageViewModel) {
    const options = { delay: 10000 };
    var toast = new window.bootstrap.Toast(this.toast.nativeElement, options);
    toast.show();
    this.ngZone.run(() => this.message = msg);
  }

  /*
    Se obtiene los eventos a los que está suscrito el usuario e inician en la fecha actual, 
    cada evento sería un grupo al que se ingresa, para recibir notificaciones y posterior
    a ello realizar alguna operación en base a los cambios de estado del evento
  */
  joinSuscriptionsGroup() {
    const id = Number(this.authService.getIdUserLocalStorage());
    this.hubService.getSuscriptionsBeforeJoinGroup(id)
      .pipe(
        takeUntil(this.stop$),
        filter(res => res.length > 0),
        map(res => res.map((x: number) => this.connectionHub.invoke("JoinGroup", x.toString())))
      ).subscribe();
  }

  /*
    Se obtiene los eventos que ha organizado el usuario e inician en la fecha actual, 
    cada evento sería un grupo al que se ingresa, para recibir notificaciones y posterior
    a ello realizar alguna operación en base a los cambios de estado del evento
  */
  joinEventsGroup() {
    const id = Number(this.authService.getIdUserLocalStorage());
    this.hubService.getEventsBeforeJoinGroup(id)
      .pipe(
        takeUntil(this.stop$),
        filter(res => res.length > 0),
        map(res => res.map((x: number) => this.connectionHub.invoke("JoinGroup", x.toString())))
      ).subscribe();
  }

  shakeBells() {
    this.bell.nativeElement.style.fill = "#FEA996";
    this.bell.nativeElement.classList.add("animate__shakeX");
    this.bell.nativeElement.addEventListener("animationend", () => {
      this.bell.nativeElement.classList.remove("animate__shakeX");
    }, { once: true });
    this.bell.nativeElement.classList.add("animate__shakeX");
  }

  resetColorBell() {
    this.bell.nativeElement.style.fill = "rgb(192, 192, 192)";
  }
}
