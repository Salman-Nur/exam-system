import {
  AfterContentChecked,
  Directive,
  ElementRef,
  Input,
  OnInit,
  Renderer2
} from "@angular/core";

@Directive({
  selector: "[ngxBootstrapInput]",
  standalone: true
})
export class NgxBootstrapInputDirective implements OnInit, AfterContentChecked {
  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) {}

  @Input() placeholderValue: string | undefined;
  @Input() idValue: string | undefined;
  @Input({ required: true }) autoCompleteValue: boolean = false;
  @Input({ required: true }) isInInvalidStateValue: boolean | undefined;
  @Input({ required: true }) typeValue: "text" | "email" | "number" | undefined;

  private initialBorderColor: string | undefined;
  private initialBorderColorApplied = true;

  ngOnInit(): void {
    const elem = this.el.nativeElement;

    this.initialBorderColor = elem.style.borderColor;
    this.renderer.addClass(elem, "form-control");

    this.renderer.setAttribute(elem, "autocomplete", this.autoCompleteValue ? "on" : "off");

    if (this.placeholderValue) {
      this.renderer.setAttribute(elem, "placeholder", this.placeholderValue);
    }

    if (this.idValue) {
      this.renderer.setAttribute(elem, "id", this.idValue);
    }

    if (this.typeValue) {
      this.renderer.setAttribute(elem, "type", this.typeValue);
    }
  }

  ngAfterContentChecked(): void {
    if (this.isInInvalidStateValue) {
      this.renderer.setStyle(this.el.nativeElement, "border-color", "#d63939");
      this.initialBorderColorApplied = false;
    } else if (!this.isInInvalidStateValue && !this.initialBorderColorApplied) {
      this.renderer.setStyle(this.el.nativeElement, "border-color", this.initialBorderColor);
      this.initialBorderColorApplied = true;
    }
  }
}
