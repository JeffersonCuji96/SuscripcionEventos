import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TokenPasswordViewModel } from 'src/app/core/models/view-models/tokenPasswordViewModel';
import { Helpers } from 'src/app/helpers/helper';
import { ValidationCustom } from 'src/app/modules/user/utils/validation-custom';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-change-password-page',
  templateUrl: './change-password-page.component.html',
  styleUrls: ['./change-password-page.component.css']
})
export class ChangePasswordPageComponent implements OnInit {

  private stop$ = new Subject<void>();
  public form: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private helper: Helpers,
    private authService: AuthService,
    private router: Router,
    private actRoute: ActivatedRoute) {
    this.form = this.formBuilder.group({
      Token: [''],
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
    this.form.enable();
    this.actRoute.params.subscribe(
      (params: Params) => {
        this.form.controls['Token'].setValue(params.token);
      }
    );
  }

  ngOnDestroy() {
    this.form.reset();
    this.stop$.next();
    this.stop$.complete();
  }

  get f(): { [key: string]: AbstractControl } {
    return this.form.controls;
  }

  changePassword() {
    this.form.disable();
    this.helper.disableInputElement("btnChangePassword", true);
    var tokenPassViewModel: TokenPasswordViewModel = this.form.value;
    this.authService.changePassword(tokenPassViewModel)
      .pipe(takeUntil(this.stop$))
      .subscribe(response => {
        this.helper.disableInputElement("btnChangePassword", false);
        this.helper.swalShowSuccess(response.Message);
        this.router.navigate(['/auth']);
      },
        error => {
          this.helper.manageErrors(error);
          this.helper.disableInputElement("btnChangePassword", false);
          this.form.enable();
        });
  }
}
