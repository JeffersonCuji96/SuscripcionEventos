import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from './home-routing.module';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';
import { PipeModule } from 'src/app/pipe.module';
import { ListaEventosComponent } from '../evento/pages/lista-eventos/lista-eventos.component';
import { DirectiveLoadData } from '../evento/utils/directive-loadData';

@NgModule({
  declarations: [
    HomePageComponent,
    ListaEventosComponent,
    DirectiveLoadData
  ],
  imports: [
    CommonModule,
    HomeRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    PipeModule
  ]
})
export class HomeModule { }
