import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from "@angular/forms";
import { NgIf } from "@angular/common";
import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { PasswordInputComponent } from "../../../../shared/others/components/password-input/password-input.component";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import { AccountService } from "../../services/account.service";
import { Router, RouterLink } from "@angular/router";
import { EMAIL_NEED_ACTIVATION } from "../../others/auth.constants";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { ToastrService } from "ngx-toastr";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { finalize, Subscription } from "rxjs";

@Component({
  selector: "app-confirm-account",
  standalone: true,
  imports: [
    FormsModule,
    NgIf,
    NgxBootstrapInputDirective,
    PasswordInputComponent,
    RouterLink,
    ReactiveFormsModule,
    RecaptchaV3Module
  ],
  templateUrl: "./confirm-account.component.html",
  styleUrl: "./confirm-account.component.scss"
})
export class ConfirmAccountComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  readonly formBuilder = inject(NonNullableFormBuilder);
  readonly accountService = inject(AccountService);
  readonly router = inject(Router);
  private readonly toastrService = inject(ToastrService);
  readonly $serverSideErrors = signal<Array<string>>([]);
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  private singleExecutionSubscription: Subscription | undefined;
  private readonly recaptchaV3ResponseCode = signal<string | null>(null);
  private renderer = inject(Renderer2);
  readonly confirmAccountForm = this.formBuilder.group({
    email: [this.emailDefaultValue, [Validators.required, Validators.email]],
    code: ["", [Validators.required]]
  });

  get email() {
    return this.confirmAccountForm.controls.email;
  }

  get code() {
    return this.confirmAccountForm.controls.code;
  }

  get emailDefaultValue() {
    const email = sessionStorage.getItem(EMAIL_NEED_ACTIVATION);
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
    if (!this.confirmAccountForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("confirmAccountForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private async callApi(token: string) {
    const res = await this.accountService.confirmAccount({
      email: this.email.value,
      code: this.code.value,
      recaptchaV3ResponseCode: this.recaptchaV3ResponseCode() ?? "placeholder"
    });

    if (res === "ok") {
      await this.router.navigateByUrl(absoluteRoutes.LOGIN);
      this.toastrService.success("Please login to continue", "Account successfully verified");
      sessionStorage.removeItem(EMAIL_NEED_ACTIVATION);
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
