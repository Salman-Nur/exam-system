import { environment } from "../../../../environments/environment";
import { HttpClient, HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { firstValueFrom } from "rxjs";
import { computed, inject, Injectable, signal } from "@angular/core";
import { SignupModel } from "../models/signup.model";
import { LoginModel } from "../models/login.model";
import {
  HttpFailedOutcome,
  HttpSuccessOutcome,
  IdentityErrorResponse,
  PlainErrorResponse,
  SignupResponse,
  ValidationErrorResponse
} from "../models/http-responses.model";
import { ConfirmAccountModel } from "../models/confirm-account.model";
import { AuthClaim, User } from "../others/auth.models";
import { XSRF_TOKEN, XSRF_TOKEN_HEADER } from "../others/auth.constants";
import { ResendVerificationCodeModel } from "../models/resend-verification-code.model";
import { ForgotPasswordModel } from "../models/forgot-password.model";
import { ResetPasswordModel } from "../models/reset-password.model";

@Injectable({
  providedIn: "root"
})

export class AccountService {
  private readonly httpClient = inject(HttpClient);
  private readonly $user = signal<User | null>(null);
  readonly $$currentUser = this.$user.asReadonly();
  readonly $$isLoggedIn = computed(() => this.$user() != null);
  readonly $$isLoggedOut = computed(() => this.$user() === null);

  public async checkLoggedInFromJwt(): Promise<boolean> {
    const user = await this.jwtCookieToUser();
    return user != null;
  }

  public async checkLoggedOutFromJwt(): Promise<boolean> {
    const user = await this.jwtCookieToUser();
    return user === null;
  }

  async login(dto: LoginModel): Promise<HttpSuccessOutcome.Ok | PlainErrorResponse> {
    try {
      await this.loadXsrfToken(false);
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.LOGIN, dto, { observe: "response" })
      );
      if (res.ok) {
        await this.loadXsrfToken(false);
        return HttpSuccessOutcome.Ok;
      }
      return new PlainErrorResponse("Something went wrong");
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (errResponse.status === HttpStatusCode.Unauthorized) {
          return new PlainErrorResponse("Invalid attempt");
        }
      }
      return new PlainErrorResponse("Something went wrong");
    }
  }

  async signUp(dto: SignupModel): Promise<SignupResponse> {
    try {
      await this.loadXsrfToken();
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.SIGN_UP, dto, { observe: "response" })
      );

      if (res.ok) {
        return HttpSuccessOutcome.Ok;
      }

      return HttpFailedOutcome.InternalServerError;
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (errResponse.status == HttpStatusCode.Unauthorized) {
          return new PlainErrorResponse(errResponse.error);
        }

        if (errResponse.status == HttpStatusCode.Conflict) {
          return HttpFailedOutcome.Conflict;
        }

        if (errResponse.error.hasOwnProperty("errors")) {
          return new IdentityErrorResponse(errResponse.error["errors"]);
        }

        if (errResponse.error instanceof Array) {
          return new ValidationErrorResponse(errResponse.error);
        }
      }
      return HttpFailedOutcome.InternalServerError;
    }
  }

  async confirmAccount(dto: ConfirmAccountModel): Promise<"ok" | string> {
    try {
      await this.loadXsrfToken();
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.CONFIRM_ACCOUNT, dto, { observe: "response" })
      );

      if (res.ok) {
        return "ok";
      }

      return "Something went wrong";
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (errResponse.status == HttpStatusCode.BadRequest) {
          return "Invalid attempt";
        }
      }

      return "Something went wrong";
    }
  }

  async resendVerificationCode(dto: ResendVerificationCodeModel): Promise<"ok" | string> {
    try {
      await this.loadXsrfToken();
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.RESEND_VERIFICATION_CODE, dto, { observe: "response" })
      );

      if (res.ok) {
        return "ok";
      }

      return "Something went wrong";
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (errResponse.status == HttpStatusCode.BadRequest) {
          return "Invalid attempt";
        }
      }

      return "Something went wrong";
    }
  }

  async forgotPassword(dto: ForgotPasswordModel): Promise<HttpSuccessOutcome.Ok | string> {
    try {
      await this.loadXsrfToken();
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.FORGOT_PASSWORD, dto, { observe: "response" })
      );

      if (res.ok) {
        return HttpSuccessOutcome.Ok;
      }

      return "Something went wrong";
    } catch (errResponse) {
      return "Something went wrong";
    }
  }

  async resetPassword(dto: ResetPasswordModel): Promise<"ok" | string> {
    try {
      await this.loadXsrfToken();
      const res = await firstValueFrom(
        this.httpClient.post(environment.V1.RESET_PASSWORD, dto, { observe: "response" })
      );

      if (res.ok) {
        return "ok";
      }

      return "Something went wrong";
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (errResponse.status == HttpStatusCode.BadRequest) {
          return "Invalid attempt";
        }
      }
      return "Something went wrong";
    }
  }

  public async logout(): Promise<boolean> {
    try {
      await this.loadXsrfToken();
      await firstValueFrom(this.httpClient.post<boolean>(environment.V1.LOGOUT, null));
      this.$user.set(null);
      sessionStorage.removeItem(XSRF_TOKEN);
      return true;
    } catch (err) {
      return false;
    }
  }

  public async loadXsrfToken(loadFromCacheIfExists = true): Promise<boolean> {
    try {
      if (loadFromCacheIfExists && sessionStorage.getItem(XSRF_TOKEN)) {
        return true;
      }
      const res = await firstValueFrom(
        this.httpClient.post<void>(environment.V1.LOAD_XSRF_TOKEN, null, { observe: "response" })
      );
      const xsrfToken = res.headers.get(XSRF_TOKEN_HEADER);
      if (xsrfToken) {
        sessionStorage.setItem(XSRF_TOKEN, xsrfToken);
        return true;
      }
      return false;
    } catch (err) {
      return false;
    }
  }

  private async jwtCookieToUser(): Promise<User | null> {
    try {
      await this.loadXsrfToken();
      const user = await firstValueFrom(
        this.httpClient.post<AuthClaim[]>(environment.V1.CHECK_TOKEN, null)
      );

      if (user) {
        const parsedUser = this.parseUserFromJwt(user);
        this.$user.set(parsedUser);
        return parsedUser;
      }

      return null;
    } catch (err) {
      this.$user.set(null);
      return null;
    }
  }

  private parseUserFromJwt(claims: Array<AuthClaim>): User {
    const data: Record<string, string> = {};

    claims.forEach((item) => {
      data[item.type] = item.value;
    });

    return {
      id: data?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ?? null,
      name: data?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ?? null,
      email: data?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] ?? null,
      profilePicture: null,
      member: data?.["Member"] === "True",
      internalUser: data?.["InternalUser"] === "True",
      manageMemberClaim: data?.["ManageMemberClaim"] === "True",
      manageMember: data?.["ManageMember"] === "True",
      manageQuestion: data?.["ManageQuestion"] === "True",
      questionCreate: data?.["QuestionCreate"] === "True",
      questionView: data?.["QuestionView"] === "True",
      questionEdit: data?.["QuestionEdit"] === "True",
      questionDelete: data?.["QuestionDelete"] === "True",
      manageExam: data?.["ManageExam"] === "True",
      examCreate: data?.["ExamCreate"] === "True",
      examView: data?.["ExamView"] === "True",
      examEdit: data?.["ExamEdit"] === "True",
      examDelete: data?.["ExamDelete"] === "True",
      manageLog: data?.["ManageLog"] === "True"
    };
  }
}
