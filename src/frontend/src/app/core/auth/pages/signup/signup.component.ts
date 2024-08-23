import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from "@angular/forms";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import { PasswordInputComponent } from "../../../../shared/others/components/password-input/password-input.component";
import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { AccountService } from "../../services/account.service";
import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { NgIf } from "@angular/common";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import {
  HttpFailedOutcome,
  HttpSuccessOutcome,
  IdentityErrorResponse,
  PlainErrorResponse,
  ValidationErrorResponse
} from "../../models/http-responses.model";
import { EMAIL_NEED_ACTIVATION } from "../../others/auth.constants";
import { LoadingSpinnerComponent } from "../../../../shared/loading-spinner/component/loading-spinner.component";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { ToastrService } from "ngx-toastr";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { finalize, Subscription } from "rxjs";

@Component({
  selector: "app-signup",
  standalone: true,
  imports: [
    ReactiveFormsModule,
    PasswordInputComponent,
    RouterModule,
    NgIf,
    NgxBootstrapInputDirective,
    LoadingSpinnerComponent,
    RecaptchaV3Module
  ],
  templateUrl: "./signup.component.html",
  styleUrl: "./signup.component.scss"
})
export class SignupComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  private readonly toastrService = inject(ToastrService);
  readonly $serverSideErrors = signal<Array<string>>([]);
  readonly formBuilder = inject(NonNullableFormBuilder);
  readonly accountService = inject(AccountService);
  private router = inject(Router);
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  private singleExecutionSubscription: Subscription | undefined;
  private renderer = inject(Renderer2);

  readonly signupForm = this.formBuilder.group(
    {
      fullName: ["", [Validators.required, Validators.maxLength(100)]],
      email: ["", [Validators.required, Validators.email]],
      password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(256)]],
      confirmPassword: ["", [Validators.required]]
    },
    { validators: [this.samePasswordValidator] }
  );

  get fullName() {
    return this.signupForm.controls.fullName;
  }

  get email() {
    return this.signupForm.controls.email;
  }

  get password() {
    return this.signupForm.controls.password;
  }

  get confirmPassword() {
    return this.signupForm.controls.confirmPassword;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, "needs-recaptcha");
  }

  ngOnDestroy(): void {
    this.unsubscribeRecaptcha();
    this.renderer.removeClass(document.body, "needs-recaptcha");
  }

  async onSubmit() {
    if (!this.signupForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("signupForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private async callApi(token: string) {
    const res = await this.accountService.signUp({
      fullName: this.fullName.value,
      email: this.email.value,
      password: this.password.value,
      confirmPassword: this.confirmPassword.value,
      recaptchaV3ResponseCode: token
    });
    if (res === HttpSuccessOutcome.Ok) {
      sessionStorage.setItem(EMAIL_NEED_ACTIVATION, this.email.value);
      await this.router.navigateByUrl(this.$$absoluteRoutes().CONFIRM_ACCOUNT);
      this.toastrService.success("Please active your account", "Code successfully sent to email");
    }

    // one-liner error
    else if (res instanceof PlainErrorResponse) {
      this.$serverSideErrors.set([res.error]);
    }

    // email is duplicate
    else if (res === HttpFailedOutcome.Conflict) {
      this.email.setErrors({ ...this.email.errors, uniqueEmail: true });
    }
    // asp-dot-net identity error
    else if (res instanceof IdentityErrorResponse) {
      this.$serverSideErrors.set(res.errors);
    }

    // model or fluent-validation error
    else if (res instanceof ValidationErrorResponse) {
      const allErrors: string[] = res.errors.reduce((acc, current) => {
        return acc.concat(current.errors);
      }, [] as string[]);

      this.$serverSideErrors.set(allErrors);
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
