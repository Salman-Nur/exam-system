import { Routes } from "@angular/router";
import { LoginComponent } from "./pages/login/login.component";
import { AlreadyLoggedInComponent } from "../error/pages/already-logged-in/already-logged-in.component";
import { sharedFallbackRoutes } from "../../shared/others/misc/shared-fallback.routes";
import { authPageGuard } from "./guards/auth-page.guard";
import { SignupComponent } from "./pages/signup/signup.component";
import { ResetPasswordComponent } from "./pages/reset-password/reset-password.component";
import { ConfirmAccountComponent } from "./pages/confirm-account/confirm-account.component";
import { ResendVerificationCodeComponent } from "./pages/resend-verification-code/resend-verification-code.component";
import { ForgotPasswordComponent } from "./pages/forgot-password/forgot-password.component";
import { authGuard } from "./guards/auth.guard";

export const authRoutes: Routes = [
  {
    path: "login",
    canActivate: [authPageGuard],
    component: LoginComponent,
    title: "Login"
  },
  {
    path: "signup",
    canActivate: [authPageGuard],
    component: SignupComponent,
    title: "Registration"
  },
  {
    path: "confirm",
    canActivate: [authPageGuard],
    component: ConfirmAccountComponent,
    title: "Confirm account"
  },
  {
    path: "forgot-password",
    canActivate: [authPageGuard],
    component: ForgotPasswordComponent,
    title: "Forgot password"
  },
  {
    path: "reset-password",
    canActivate: [authPageGuard],
    component: ResetPasswordComponent,
    title: "Reset password"
  },

  {
    path: "resend-verification-code",
    canActivate: [authPageGuard],
    component: ResendVerificationCodeComponent,
    title: " Resend verification code"
  },
  {
    path: "already-authenticated",
    canActivate: [authGuard],
    component: AlreadyLoggedInComponent,
    title: "Already logged in"
  },
  ...sharedFallbackRoutes
];
