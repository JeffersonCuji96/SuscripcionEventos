import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventoRoutingModule } from './evento-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BuilderService } from './services/builder.service';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { DirectiveFormatTime } from './utils/directive-formatTime';
import { DetailEventoComponent } from './pages/detail-evento/detail-evento.component';
import { PipeModule } from 'src/app/pipe.module';
import { ReviewEventosComponent } from './pages/review-eventos/review-eventos.component';
import { SimpleModalModule } from 'ngx-simple-modal';
import { LazyLoadImageModule } from 'ng-lazyload-image';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    RegisterPageComponent,
    DetailEventoComponent,
    DirectiveFormatTime,
    ReviewEventosComponent
  ],
  imports: [
    CommonModule,
    EventoRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    PipeModule,
    SimpleModalModule,
    SharedModule,
    LazyLoadImageModule
  ],
  exports:[
    DetailEventoComponent
  ],
  entryComponents:[
    RegisterPageComponent
  ],
  providers:[ BuilderService ]
})
export class EventoModule { }
