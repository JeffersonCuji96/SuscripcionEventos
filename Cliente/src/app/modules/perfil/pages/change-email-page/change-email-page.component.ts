import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SimpleModalComponent } from 'ngx-simple-modal';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AlertModel } from 'src/app/core/models/alert-model';
import { UserEmailViewModel } from 'src/app/core/models/view-models/userEmailViewModel';
import { Helpers } from 'src/app/helpers/helper';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { UserService } from 'src/app/modules/user/services/user.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { PerfilService } from '../../services/perfil.service';

@Component({
  selector: 'app-change-email-page',
  templateUrl: './change-email-page.component.html',
  styleUrls: ['./change-email-page.component.css']
})
export class ChangeEmailPageComponent extends SimpleModalComponent<AlertModel, null> implements AlertModel, OnInit, OnDestroy {

  Title: string;
  Data: any;
  private stop$ = new Subject<void>();
  public form: FormGroup;
  public btnId = "btnUpdateEmail";

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private perfilService:PerfilService,
    private authService:AuthService,
    private router:Router,
    private helper: Helpers) {
    super();
    this.form = this.formBuilder.group({
      Id:[this.authService.getIdUserLocalStorage()],
      Email: ['', {
        validators: [Validators.compose([
          Validators.required,
          ValidationCustom.isEmailFormat,
          Validators.pattern(/^[a-zA-Z0-9ñÑ.@]+$/)
        ])]
      }],
      ClaveActual:['',Validators.required]
    });
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.form.enable();
  }

  ngOnDestroy() {
    this.form.reset();
    this.stop$.next();
    this.stop$.complete();
  }

  availableInfo(ctrl: AbstractControl, fnValidator: AsyncValidatorFn): void {
    ctrl.setAsyncValidators(fnValidator);
    ctrl.updateValueAndValidity();
    ctrl.clearAsyncValidators();
  }

  availableEmail(): void {
    this.availableInfo(
      this.f.Email,
      ValidationCustom.isAvailableEmail(this.stop$, this.userService)
    );
  }

  updateEmail() {
    this.form.disable();
    this.helper.disableInputElement(this.btnId, true);
    var userEmail: UserEmailViewModel = this.form.value;
    this.perfilService.updateEmail(userEmail)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.swalShowSuccess(response.Message);
        this.close();
        this.authService.removeSesion();
        this.router.navigate(['/', 'auth'])
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnId, false);
          this.form.enable();
          this.close();
        });
  }
}
