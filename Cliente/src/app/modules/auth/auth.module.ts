import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ForgotPageComponent } from './pages/forgot-page/forgot-page.component';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { LoaderComponent } from 'src/app/shared/components/loader/loader.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    LoginPageComponent,
    ForgotPageComponent,
    ChangePasswordPageComponent,
    ConfirmEmailComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModule
  ]
})
export class AuthModule { }
