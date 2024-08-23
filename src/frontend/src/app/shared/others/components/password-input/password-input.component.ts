import { NgClass, NgStyle } from "@angular/common";
import { Component, Input } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

@Component({
  selector: "bootstrap-password-input",
  standalone: true,
  imports: [NgStyle, NgClass],
  templateUrl: "./password-input.component.html",
  styleUrl: "./password-input.component.scss",
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: PasswordInputComponent,
      multi: true
    }
  ]
})
export class PasswordInputComponent implements ControlValueAccessor {
  password: string = "";
  disabled: boolean = false;
  @Input({ required: true }) isInErrorState: boolean = false;
  @Input() placeholderText: string = "Enter password";
  @Input({ required: true }) idValue: string | undefined;
  isPasswordVisible: boolean = false;

  writeValue(value?: string): void {
    if (value) {
      this.password = value;
    }
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onPasswordChange(elem: HTMLInputElement): void {
    this.password = elem.value;
    this.onChange(this.password);
  }

  onBlur(): void {
    this.onTouched();
  }

  togglePasswordVisibility(event: MouseEvent, button: HTMLInputElement) {
    event.preventDefault();

    if (button.type === "password") {
      button.type = "text";
      this.isPasswordVisible = true;
    } else {
      button.type = "password";
      this.isPasswordVisible = false;
    }
  }

  private onChange: (value: string) => void = () => {};

  private onTouched: () => void = () => {};
}
