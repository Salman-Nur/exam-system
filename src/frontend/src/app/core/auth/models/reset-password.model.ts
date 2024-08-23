export interface ResetPasswordModel {
  email: string;
  password: string;
  confirmPassword: string;
  code: string;
  recaptchaV3ResponseCode: string;
}
