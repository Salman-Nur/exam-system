import { Directive, ElementRef, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appButton]',
  standalone: true
})
export class ButtonDirective {

  constructor(private element: ElementRef, private renderer: Renderer2) { 
    this.renderer.addClass(this.element.nativeElement, 'btn');
    this.renderer.addClass(this.element.nativeElement, 'btn-outline-primary');
    this.renderer.addClass(this.element.nativeElement, 'btn-md');
    this.renderer.addClass(this.element.nativeElement, 'btn-block');
    this.renderer.addClass(this.element.nativeElement, 'mt-3');
  }

}
