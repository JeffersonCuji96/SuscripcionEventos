import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { ErrorRoutingModule } from './error-routing.module';
import { Error404Component } from './pages/error404/error404.component';
import { RecoveryErrorComponent } from './pages/recovery-error/recovery-error.component';
import { Error401Component } from './pages/error401/error401.component';
import { Error500Component } from './pages/error500/error500.component';
import { ConfirmErrorComponent } from './pages/confirm-error/confirm-error.component';
import { FailedErrorComponent } from './pages/failed-error/failed-error.component';

@NgModule({
  declarations: [
    Error404Component,
    RecoveryErrorComponent,
    Error401Component,
    Error500Component,
    ConfirmErrorComponent,
    FailedErrorComponent],
  imports: [
    CommonModule,
    ErrorRoutingModule,
    ReactiveFormsModule,
    SharedModule
  ]
})
export class ErrorModule { }
