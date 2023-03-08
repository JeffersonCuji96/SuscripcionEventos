import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SimpleModalComponent } from 'ngx-simple-modal';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AlertModel } from 'src/app/core/models/alert-model';
import { UserPasswordViewModel } from 'src/app/core/models/view-models/userPasswordViewModel';
import { Helpers } from 'src/app/helpers/helper';
import { AuthService } from 'src/app/modules/auth/services/auth.service';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { PerfilService } from '../../services/perfil.service';

@Component({
  selector: 'app-change-password-page',
  templateUrl: './change-password-page.component.html',
  styleUrls: ['./change-password-page.component.css']
})
export class ChangePasswordPageComponent extends SimpleModalComponent<AlertModel, null> implements AlertModel, OnInit, OnDestroy {
  Title: string;
  Data: any;
  private stop$ = new Subject<void>();
  public form: FormGroup;
  public btnId = "btnUpdatePassword";

  constructor(
    private formBuilder: FormBuilder,
    private authService:AuthService,
    private perfilService:PerfilService,
    private helper: Helpers) {
    super();
    this.form = this.formBuilder.group({
      Id: [this.authService.getIdUserLocalStorage()],
      Clave: ['', {
        validators: [Validators.compose([
          Validators.required,
          ValidationCustom.isValidPassword
        ])]
      }],
      ConfirmClave: ['', Validators.required],
      ClaveActual:['',Validators.required]
    }, {
      validators: [ValidationCustom.isMatchTwoCtrl("Clave", "ConfirmClave")],
    });
  }

  ngOnInit(): void {
    this.form.enable();
  }

  ngOnDestroy() {
    this.form.reset();
    this.stop$.next();
    this.stop$.complete();
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }

  updatePassword() {
    this.form.disable();
    this.helper.disableInputElement(this.btnId, true);
    var userPass: UserPasswordViewModel = this.form.value;
    this.perfilService.updatePassword(userPass)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.swalShowSuccess(response.Message);
        this.close();
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement(this.btnId, false);
          this.form.enable();
          this.close();
        });
  }

}
