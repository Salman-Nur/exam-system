import { AbstractControl, ValidatorFn } from "@angular/forms";

export function timeValidator(): ValidatorFn {
  return (control: AbstractControl) : { [key: string]: boolean } | null => {
    if(control.value >= 1 && control.value <= 10){
      return null;
    }else {
      return { 'timeValidator': true };
    }
  }
}
