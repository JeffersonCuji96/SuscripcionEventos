import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ForgotGuard } from 'src/app/core/guards/forgot.guard';
import { ChangePasswordPageComponent } from './pages/change-password-page/change-password-page.component';
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
    path: '**',
    redirectTo: '/auth/login'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
