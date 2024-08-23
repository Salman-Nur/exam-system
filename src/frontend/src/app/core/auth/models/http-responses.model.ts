export interface SingleValidationErrorResponse {
  field: string;
  errors: string[];
}

export class ValidationErrorResponse {
  constructor(readonly errors: SingleValidationErrorResponse[]) {}
}

export class IdentityErrorResponse {
  constructor(readonly errors: string[]) {}
}

export class PlainErrorResponse {
  constructor(readonly error: string) {}
}

export type SignupResponse =
  | HttpFailedOutcome
  | HttpSuccessOutcome
  | ValidationErrorResponse
  | IdentityErrorResponse
  | PlainErrorResponse;

export enum HttpFailedOutcome {
  Unauthorized = 1,
  Conflict,
  NotFound,
  BadRequest,
  Forbidden,
  InternalServerError
}

export enum HttpSuccessOutcome {
  Ok = 1,
  Created,
  NoContent
}
