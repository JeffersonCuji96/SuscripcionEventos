import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PerfilRoutingModule } from './perfil-routing.module';
import { PerfilPageComponent } from './pages/perfil-page/perfil-page.component';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
import { ChangeEmailPageComponent } from './pages/change-email-page/change-email-page.component';
import { ChangePersonalInfoPageComponent } from './pages/change-personalinfo-page/change-personalinfo-page.component';
import { SimpleModalModule } from 'ngx-simple-modal';

@NgModule({
  declarations: [
    PerfilPageComponent,
    ChangePasswordPageComponent,
    ChangeEmailPageComponent,
    ChangePersonalInfoPageComponent
  ],
  imports: [
    CommonModule,
    PerfilRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    SimpleModalModule
  ],
  entryComponents:[
    ChangePasswordPageComponent,
    ChangeEmailPageComponent,
  ],
  providers:[]
})
export class PerfilModule { }
