import { NgZone, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { HubConnection } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';
import { HubService } from 'src/app/app-services/hub.service';
import { MessageViewModel } from 'src/app/core/models/view-models/messageViewModel';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { SuscripcionService } from 'src/app/modules/suscripcion/services/suscripcion.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  public fullName: string;
  public message: MessageViewModel = null;
  maxWidth = 992;
  @ViewChild('menuDesplegable') menuDesplegable!: ElementRef;
  private connectionHub: HubConnection;
  @ViewChild('toast') toast: ElementRef;
  private stop$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private hubService: HubService,
    private ngZone: NgZone,
    private suscriptionService:SuscripcionService
  ) {
    this.fullName = authService.getFullName();
    this.connectionHub = this.hubService.getConnectionHub();
  }

  ngOnInit(): void {
    this.checkSuscriptionEmitChanges();
    this.startHubNotification();
  }
  
  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  startHubNotification(){
    this.connectionHub.start().then(_ => {
      this.joinGroup();
      this.connectionHub.on("Notification", msg =>{ 
        this.suscriptionService.suscriptionChange.emit(true);
        this.showNotification(msg);
      });
    });
  }

  logout() {
    this.closeToggler();
    this.authService.removeSesion();
    window.location.href = "/auth/login";
  }

  closeToggler(): void {
    const windowWidth = window.innerWidth;
    if (windowWidth < this.maxWidth) {
      this.menuDesplegable.nativeElement.click();
    }
  }

  showNotification(msg:MessageViewModel) {
    const options = { delay: 10000 };
    var toast = new window.bootstrap.Toast(this.toast.nativeElement, options);
    toast.show();
    this.ngZone.run(() => this.message=msg);
  }

  joinGroup() {
    const id = Number(this.authService.getIdUserLocalStorage());
    this.hubService.getEventsBeforeJoinGroup(id)
      .pipe(
        takeUntil(this.stop$),
        filter(res => res.length > 0),
        map(res => res.map((x: number) => this.connectionHub.invoke("JoinGroup", x.toString())))
      ).subscribe();
  }

  
  checkSuscriptionEmitChanges() {
    this.suscriptionService.suscriptionChange.pipe(
      takeUntil(this.stop$))
      .subscribe(res => {
        res ? this.joinGroup() : null;
      });
  }
}
