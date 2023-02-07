import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';

@Component({
  selector: 'app-change-password-page',
  templateUrl: './change-password-page.component.html',
  styleUrls: ['./change-password-page.component.css']
})
export class ChangePasswordPageComponent implements OnInit {

  private stop$ = new Subject<void>();
  public form: FormGroup;

  constructor(
    private formBuilder: FormBuilder) {
    this.form = this.formBuilder.group({
      Clave: ['', {
        validators: [Validators.compose([
          Validators.required,
          ValidationCustom.isValidPassword
        ])]
      }],
      ConfirmClave: ['', Validators.required],
    }, {
      validators: [ValidationCustom.isMatchTwoCtrl("Clave", "ConfirmClave")],
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }

}
