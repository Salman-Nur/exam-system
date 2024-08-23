import { Directive, HostBinding, Input } from "@angular/core";

@Directive({
  selector: "[inputFieldErrorBorder]",
  standalone: true
})
export class InputFieldErrorBorderDirective {
  @Input({ alias: "inputFieldErrorBorder", required: true })
  showInvalidBorder: boolean = false;

  @HostBinding("class.form-is-invalid")
  get cssClasses() {
    return this.showInvalidBorder;
  }
}
