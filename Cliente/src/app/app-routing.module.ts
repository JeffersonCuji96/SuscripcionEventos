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
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
