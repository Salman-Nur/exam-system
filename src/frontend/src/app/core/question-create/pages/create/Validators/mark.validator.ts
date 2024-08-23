import { AbstractControl, ValidatorFn } from "@angular/forms";

export function markValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const isValid = control.value > 0;
    return isValid ? null : { invalidMark: { value: control.value } };
  };
}
