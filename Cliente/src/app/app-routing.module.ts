import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SessionGuard } from './core/guards/session.guard';
import { HomePageComponent } from './modules/home/pages/home-page/home-page.component';
 
const routes: Routes = [
  {
    path: '',
    component:HomePageComponent,
    loadChildren: () => import(`./modules/home/home.module`).then(m => m.HomeModule),
    canActivate: [SessionGuard],
  },
  {
    path: 'auth', 
    loadChildren: () => import(`./modules/auth/auth.module`).then(m => m.AuthModule)
  },
  {
    path:'user',
    loadChildren:()=>import(`./modules/user/user.module`).then(m=>m.UserModule)
  },
  {
    path:'error',
    loadChildren:()=>import(`./modules/error/error.module`).then(m=>m.ErrorModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
