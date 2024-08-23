import { Injectable } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";

@Injectable({
  providedIn: "root"
})
export class ReactiveFormUtilsService {
  hasValidationError(formControl: FormControl): boolean {
    return (formControl.invalid && (formControl.dirty || formControl.touched)) ?? false;
  }

  showValidationErrorBorder(formControl: FormControl): boolean {
    return this.hasValidationError(formControl);
  }

  disableFormSubmission(form: FormGroup): boolean {
    return !form.valid;
  }
}
