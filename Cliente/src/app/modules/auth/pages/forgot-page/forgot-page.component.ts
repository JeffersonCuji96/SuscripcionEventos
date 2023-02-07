import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { UserService } from 'src/app/modules/user/services/user.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';

@Component({
  selector: 'app-forgot-page',
  templateUrl: './forgot-page.component.html',
  styleUrls: ['./forgot-page.component.css']
})
export class ForgotPageComponent implements OnInit,OnDestroy {

  private stop$ = new Subject<void>();
  public form: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private userService:UserService) { 
      this.form = this.formBuilder.group({
        Email: ['', {
          validators: [Validators.compose([
            Validators.required,
            ValidationCustom.isEmailFormat,
            Validators.pattern(/^[a-zA-Z0-9ñÑ.@]+$/)
          ])]
        }]
      });
    }

  ngOnInit(): void {}

  ngOnDestroy() {
    this.stop$.next();
    this.stop$.complete();
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }
}
