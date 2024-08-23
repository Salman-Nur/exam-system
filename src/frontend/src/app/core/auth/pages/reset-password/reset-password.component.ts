import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from "@angular/forms";
import { AccountService } from "../../services/account.service";
import { Router, RouterLink } from "@angular/router";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { PasswordInputComponent } from "../../../../shared/others/components/password-input/password-input.component";
import { NgIf } from "@angular/common";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { ToastrService } from "ngx-toastr";
import { EMAIL_NEED_PASSWORD_RESET } from "../../others/auth.constants";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { finalize, Subscription } from "rxjs";

@Component({
  selector: "app-reset-password",
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NgxBootstrapInputDirective,
    RouterLink,
    PasswordInputComponent,
    NgIf,
    RecaptchaV3Module
  ],
  templateUrl: "./reset-password.component.html",
  styleUrl: "./reset-password.component.scss"
})
export class ResetPasswordComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly toastrService = inject(ToastrService);
  readonly accountService = inject(AccountService);
  readonly router = inject(Router);
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  private singleExecutionSubscription: Subscription | undefined;
  private renderer = inject(Renderer2);
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
  readonly $serverSideErrors = signal<Array<string>>([]);

  readonly resetPasswordForm = this.formBuilder.group(
    {
      email: [this.emailDefaultValue, [Validators.required, Validators.email]],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(256)]],
      confirmPassword: ["", [Validators.required]],
      code: ["", [Validators.required]]
    },
    { validators: [this.samePasswordValidator] }
  );

  get email() {
    return this.resetPasswordForm.controls.email;
  }

  get password() {
    return this.resetPasswordForm.controls.password;
  }

  get confirmPassword() {
    return this.resetPasswordForm.controls.confirmPassword;
  }

  get code() {
    return this.resetPasswordForm.controls.code;
  }

  get emailDefaultValue() {
    const email = sessionStorage.getItem(EMAIL_NEED_PASSWORD_RESET);
    return email ? email : "";
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, "needs-recaptcha");
  }

  ngOnDestroy(): void {
    this.unsubscribeRecaptcha();
    this.renderer.removeClass(document.body, "needs-recaptcha");
  }

  async onSubmit() {
    if (!this.resetPasswordForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("resetPasswordForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private async callApi(token: string) {
    const res = await this.accountService.resetPassword({
      email: this.email.value,
      password: this.password.value,
      confirmPassword: this.confirmPassword.value,
      code: this.code.value,
      recaptchaV3ResponseCode: token
    });
    if (res === "ok") {
      sessionStorage.removeItem(EMAIL_NEED_PASSWORD_RESET);
      await this.router.navigateByUrl(absoluteRoutes.LOGIN);
      this.toastrService.success("Please login to continue", "Password reset successful");
    } else {
      this.$serverSideErrors.set([res]);
    }
  }

  private showRecaptchaError() {
    this.toastrService.error("Refresh the page and try again", "Recaptcha Error");
  }

  private unsubscribeRecaptcha() {
    if (this.singleExecutionSubscription) {
      this.singleExecutionSubscription.unsubscribe();
    }
  }

  private samePasswordValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.valid) {
      return null;
    }
    if (control.get("password")?.value != control.get("confirmPassword")?.value) {
      return { hasIdenticalPassword: true };
    }

    return null;
  }
}
