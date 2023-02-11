import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConfirmGuard } from 'src/app/core/guards/confirm.guard';
import { ForgotGuard } from 'src/app/core/guards/forgot.guard';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { ForgotPageComponent } from './pages/forgot-page/forgot-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent
  },
  {
    path:'forgot',
    component:ForgotPageComponent
  },
  {
    path:'change-password/:token',
    canActivate: [ForgotGuard],
    component:ChangePasswordPageComponent
  },
  {
    path:'confirm-email/:token',
    canActivate:[ConfirmGuard],
    component:ConfirmEmailComponent
  },
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
