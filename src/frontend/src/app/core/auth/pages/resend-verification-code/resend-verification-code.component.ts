import { NgIf } from "@angular/common";
import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from "@angular/forms";
import { Router, RouterModule } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { PasswordInputComponent } from "../../../../shared/others/components/password-input/password-input.component";
import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import { EMAIL_NEED_ACTIVATION } from "../../others/auth.constants";
import { AccountService } from "../../services/account.service";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { finalize, Subscription } from "rxjs";

@Component({
  selector: "app-resend-verification-code",
  standalone: true,
  templateUrl: "./resend-verification-code.component.html",
  styleUrl: "./resend-verification-code.component.scss",
  imports: [
    NgIf,
    PasswordInputComponent,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBootstrapInputDirective,
    RecaptchaV3Module
  ]
})
export class ResendVerificationCodeComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  private readonly toastrService = inject(ToastrService);
  readonly $serverSideErrors = signal<Array<string>>([]);
  readonly formBuilder = inject(NonNullableFormBuilder);
  readonly accountService = inject(AccountService);
  readonly router = inject(Router);
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  private singleExecutionSubscription: Subscription | undefined;
  private renderer = inject(Renderer2);
  readonly resendCodeForm = this.formBuilder.group({
    email: ["", [Validators.required, Validators.email]],
    password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(256)]]
  });

  get email() {
    return this.resendCodeForm.controls.email;
  }

  get password() {
    return this.resendCodeForm.controls.password;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, "needs-recaptcha");
  }

  ngOnDestroy(): void {
    this.unsubscribeRecaptcha();
    this.renderer.removeClass(document.body, "needs-recaptcha");
  }

  async onSubmit() {
    if (!this.resendCodeForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("resendCodeForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private async callApi(token: string) {
    const res = await this.accountService.resendVerificationCode({
      email: this.email.value,
      password: this.password.value,
      recaptchaV3ResponseCode: token
    });

    if (res === "ok") {
      sessionStorage.setItem(EMAIL_NEED_ACTIVATION, this.email.value);
      await this.router.navigateByUrl(absoluteRoutes.CONFIRM_ACCOUNT);
      this.toastrService.success("Please active your account", "Code successfully sent to email");
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
}
