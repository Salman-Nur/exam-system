import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from "@angular/forms";
import { Router, RouterModule } from "@angular/router";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import { AccountService } from "../../services/account.service";
import { NgIf } from "@angular/common";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import { ToastrService } from "ngx-toastr";
import { EMAIL_NEED_PASSWORD_RESET } from "../../others/auth.constants";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { finalize, Subscription } from "rxjs";
import { HttpSuccessOutcome } from "../../models/http-responses.model";

@Component({
  selector: "app-forgot-password",
  standalone: true,
  imports: [
    NgIf,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBootstrapInputDirective,
    RecaptchaV3Module
  ],
  templateUrl: "./forgot-password.component.html",
  styleUrl: "./forgot-password.component.scss"
})
export class ForgotPasswordComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  private readonly toastrService = inject(ToastrService);
  readonly formBuilder = inject(NonNullableFormBuilder);
  readonly accountService = inject(AccountService);
  readonly router = inject(Router);
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  private singleExecutionSubscription: Subscription | undefined;
  readonly $serverSideErrors = signal<Array<string>>([]);
  private renderer = inject(Renderer2);
  readonly forgotPasswordForm = this.formBuilder.group({
    email: ["", [Validators.required, Validators.email]]
  });

  get email() {
    return this.forgotPasswordForm.controls.email;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, "needs-recaptcha");
  }

  ngOnDestroy(): void {
    this.unsubscribeRecaptcha();
    this.renderer.removeClass(document.body, "needs-recaptcha");
  }

  async onSubmit() {
    if (!this.forgotPasswordForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("forgotPasswordForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private showRecaptchaError() {
    this.toastrService.error("Refresh the page and try again", "Recaptcha Error");
  }

  private unsubscribeRecaptcha() {
    if (this.singleExecutionSubscription) {
      this.singleExecutionSubscription.unsubscribe();
    }
  }

  private async callApi(token: string) {
    const res = await this.accountService.forgotPassword({
      email: this.email.value,
      recaptchaV3ResponseCode: token
    });
    if (res === HttpSuccessOutcome.Ok) {
      sessionStorage.setItem(EMAIL_NEED_PASSWORD_RESET, this.email.value);
      await this.router.navigateByUrl(absoluteRoutes.RESET_PASSWORD);
      this.toastrService.success("Please reset your password", "Code successfully sent to email");
    } else {
      this.$serverSideErrors.set([res]);
    }
  }
}
