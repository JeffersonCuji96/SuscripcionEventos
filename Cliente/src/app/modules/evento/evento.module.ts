import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventoRoutingModule } from './evento-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BuilderService } from './services/builder.service';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { DirectiveFormatTime } from './utils/directive-formatTime';

@NgModule({
  declarations: [
    RegisterPageComponent,
    DirectiveFormatTime
  ],
  imports: [
    CommonModule,
    EventoRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers:[BuilderService]
})
export class EventoModule { }
