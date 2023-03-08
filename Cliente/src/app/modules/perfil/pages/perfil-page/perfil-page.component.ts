import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PersonaDto } from 'src/app/core/models/personaDto';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { UserService } from 'src/app/modules/user/services/user.service';
import { SimpleModalService } from 'ngx-simple-modal';
import { ChangePasswordPageComponent } from '../change-password-page/change-password-page.component';
import { ChangeEmailPageComponent } from '../change-email-page/change-email-page.component';

@Component({
  selector: 'app-perfil-page',
  templateUrl: './perfil-page.component.html',
  styleUrls: ['./perfil-page.component.css']
})
export class PerfilPageComponent implements OnInit,OnDestroy {

  private stop$ = new Subject<void>();
  public oPersona: PersonaDto;
  public fullName:string;

  constructor(
    private userService:UserService,
    private authService:AuthService,
    private SimpleModalService: SimpleModalService
    ) { 
      this.fullName=authService.getFullName();
    }

  ngOnInit(): void {
    this.getUserById();
  }

  openModalUpdatePassword() {
    this.SimpleModalService.addModal(
      ChangePasswordPageComponent, 
      {
        Title: 'Cambio de contraseña', 
        Data: null
      }
    );
  }
  
  openModalUpdateEmail(){
    this.SimpleModalService.addModal(
      ChangeEmailPageComponent, 
      {
        Title: 'Cambio de email', 
        Data: null
      }
    );
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  getUserById() {
    let id=this.authService.getIdUserLocalStorage();
    return this.userService.getByIdUser(id).pipe(
      takeUntil(this.stop$))
      .subscribe(
        res => {
          this.oPersona = res;
        });
  }
}
