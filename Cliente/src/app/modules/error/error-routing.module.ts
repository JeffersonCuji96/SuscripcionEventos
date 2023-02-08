import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConfirmErrorComponent } from './pages/confirm-error/confirm-error.component';
import { Error401Component } from './pages/error401/error401.component';
import { Error404Component } from './pages/error404/error404.component';
import { Error500Component } from './pages/error500/error500.component';
import { FailedErrorComponent } from './pages/failed-error/failed-error.component';
import { RecoveryErrorComponent } from './pages/recovery-error/recovery-error.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '404',
  }, 
  {
    path:'401',
    component:Error401Component
  },
  {
    path:'404',
    component:Error404Component
  },
  {
    path:'500',
    component:Error500Component
  },
  {
    path:'recovery-error',
    component:RecoveryErrorComponent
  },
  {
    path:'confirm-error',
    component:ConfirmErrorComponent
  },
  {
    path:'failed-error',
    component:FailedErrorComponent
  },
  {
    path: '**',
    pathMatch: 'full',
    component: Error404Component,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ErrorRoutingModule { }
