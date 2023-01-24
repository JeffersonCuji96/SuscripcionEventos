import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { Helpers } from 'src/app/helpers/helper';
import { BuilderService } from '../../services/builder.service';
import { UserService } from '../../services/user.service';
import { ValidationCustom } from '../../utils/validation-custom';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent implements OnInit, OnDestroy {

  private stop$ = new Subject<void>();

  constructor(public serviceBuilder: BuilderService,
    private userService: UserService, private helper: Helpers) {

  }
  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  availableEmail(): void {
    this.serviceBuilder.availableInfo(
      this.f.Email,
      ValidationCustom.isAvailableEmail(this.userService)
    );
  }

  availableDateBirth() {
    this.serviceBuilder.availableInfo(
      this.fNested('Persona.FechaNacimiento'),
      ValidationCustom.isAvailableDateBirth(this.userService)
    );
  }

  availablePhone(): void {
    this.serviceBuilder.availableInfo(
      this.fNested('Persona.Telefono'),
      ValidationCustom.isAvailablePhone(this.userService)
    );
  }

  get f(): { [key: string]: AbstractControl } {
    return this.serviceBuilder.form.controls;
  }

  fNested(name: any): FormControl {
    return <FormControl>this.serviceBuilder.form.get(name)
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

}
