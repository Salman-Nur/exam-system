import { PasswordInputComponent } from "../../../../shared/others/components/password-input/password-input.component";
import { NgxBootstrapInputDirective } from "../../../../shared/others/directives/ngx-bootstrap-input.directive";
import { ReactiveFormUtilsService } from "../../../../shared/others/services/reactive-form-utils.service";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";
import {
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from "@angular/forms";
import { NgIf } from "@angular/common";
import { Component, inject, OnDestroy, OnInit, Renderer2, signal } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { AccountService } from "../../services/account.service";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { HttpSuccessOutcome, PlainErrorResponse } from "../../models/http-responses.model";
import { finalize, Subscription } from "rxjs";
import { RecaptchaV3Module, ReCaptchaV3Service } from "ng-recaptcha-2";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [
    NgIf,
    PasswordInputComponent,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
    NgxBootstrapInputDirective,
    RecaptchaV3Module
  ],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss"
})
export class LoginComponent implements OnDestroy, OnInit {
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  readonly formBuilder = inject(NonNullableFormBuilder);
  readonly accountService = inject(AccountService);
  readonly router = inject(Router);
  private singleExecutionSubscription: Subscription | undefined;
  private readonly toastrService = inject(ToastrService);
  private renderer = inject(Renderer2);
  private recaptchaV3Service = inject(ReCaptchaV3Service);
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
  readonly $serverSideErrors = signal<Array<string>>([]);

  readonly loginForm = this.formBuilder.group({
    email: ["", [Validators.required, Validators.email]],
    password: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(256)]],
    rememberMe: [false]
  });

  get email() {
    return this.loginForm.controls.email;
  }

  get password() {
    return this.loginForm.controls.password;
  }

  get rememberMe() {
    return this.loginForm.controls.rememberMe;
  }

  ngOnInit(): void {
    this.renderer.addClass(document.body, "needs-recaptcha");
  }

  ngOnDestroy(): void {
    this.unsubscribeRecaptcha();

    this.renderer.removeClass(document.body, "needs-recaptcha");
  }

  async onSubmit() {
    if (!this.loginForm.valid) {
      return;
    }

    this.loadingSpinnerService.loadingOn();
    this.unsubscribeRecaptcha();

    this.singleExecutionSubscription = this.recaptchaV3Service
      .execute("loginForm")
      .pipe(finalize(() => this.loadingSpinnerService.loadingOff()))
      .subscribe({
        next: async (data) => await this.callApi(data),
        error: () => this.showRecaptchaError()
      });
  }

  private async callApi(token: string) {
    const res = await this.accountService.login({
      email: this.email.value,
      password: this.password.value,
      rememberMe: this.rememberMe.value,
      recaptchaV3ResponseCode: token
    });
    if (res === HttpSuccessOutcome.Ok && (await this.accountService.checkLoggedInFromJwt())) {
      await this.router.navigateByUrl(absoluteRoutes.DASHBOARD);
    }
    else if (res instanceof PlainErrorResponse) {
      this.$serverSideErrors.set([res.error]);
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
