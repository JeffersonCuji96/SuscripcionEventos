import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { Helpers } from 'src/app/helpers/helper';
import { UserService } from 'src/app/modules/user/services/user.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { BuilderService } from '../../services/builder.service';
import { EventoService } from '../../services/evento.service';
import { ValidationEventoCustom } from '../../utils/validation-custom';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();
  public isShowBoxDateTime:boolean=false;

  constructor(
    public serviceBuilder: BuilderService,
    private eventService: EventoService,
    private userService: UserService,
    private router: Router,
    private helper: Helpers) {

  }
  ngOnInit(): void {
    this.serviceBuilder.form.enable();
  }

  ngOnDestroy() {
    this.serviceBuilder.form.reset();
    this.stop$.next();
    this.stop$.complete();
  }


  get f(): { [key: string]: AbstractControl } {
    return this.serviceBuilder.form.controls;
  }

  onFileChanged(event: any) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      var valid = ValidationCustom.isAvailableFormatAndSize(file, this.f.FileTemporal);
      if (valid === null) {
        this.helper.convertFileBase64(file).subscribe(base64 => {
          this.serviceBuilder.form.patchValue({
            ImageBase64: base64,
          });
        });
      }
    }
  }

  showBoxDateTime(value:any){
    this.isShowBoxDateTime=value;
    if(value===false){
      this.f.FechaFin.setValue(null);
      this.f.HoraFin.setValue(null);
      this.serviceBuilder.removeValidators(this.f.HoraFin);
      this.serviceBuilder.removeValidators(this.f.FechaFin);
    }else{
      this.serviceBuilder.addValidRequired(this.f.HoraFin);
      this.serviceBuilder.addValidRequired(this.f.FechaFin);
    }
  }

  availableDateStart() {
    this.serviceBuilder.availableInfo(
      this.f.FechaInicio,
      ValidationEventoCustom.isAvailableDateStart(this.userService)
    );
  }
}
