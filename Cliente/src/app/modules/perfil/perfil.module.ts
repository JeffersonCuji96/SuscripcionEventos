import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PerfilRoutingModule } from './perfil-routing.module';
import { PerfilPageComponent } from './pages/perfil-page/perfil-page.component';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
import { ChangeEmailPageComponent } from './pages/change-email-page/change-email-page.component';
import { ChangePersonalInfoPageComponent } from './pages/change-personalinfo-page/change-personalinfo-page.component';
import { SimpleModalModule } from 'ngx-simple-modal';
import { SharedModule } from 'src/app/shared/shared.module';
import { PipeModule } from 'src/app/pipe.module';
import { LazyLoadImageModule } from 'ng-lazyload-image';

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
    SimpleModalModule,
    SharedModule,
    PipeModule,
    LazyLoadImageModule
  ],
  entryComponents:[
    ChangePasswordPageComponent,
    ChangeEmailPageComponent,
    ChangePersonalInfoPageComponent
  ]
})
export class PerfilModule { }
