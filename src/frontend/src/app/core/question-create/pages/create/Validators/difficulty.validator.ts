import { AbstractControl, Validator, ValidatorFn } from "@angular/forms";

export function difficultyValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: boolean } | null => {
    if (control.value > 0 && control.value < 6) {
      return null; // Validation passes
    } else {
      return { 'difficultyValidator': true };
    }
  };
}
