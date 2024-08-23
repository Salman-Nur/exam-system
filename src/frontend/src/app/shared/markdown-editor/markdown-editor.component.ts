import EasyMDE, { Options } from "easymde";
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
  ViewChild
} from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

@Component({
  selector: "markdown-editor",
  standalone: true,
  templateUrl: "./markdown-editor.component.html",
  styleUrl: "./markdown-editor.component.scss",
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: MarkdownEditorComponent,
      multi: true
    }
  ]
})
export class MarkdownEditorComponent implements AfterViewInit, OnDestroy, ControlValueAccessor {

  private onChange: (value: string) => void = () => {};

  private onTouched: () => void = () => {};
  disabled: boolean = false;

  writeValue(value?: string): void {
    this.easyMdeInstance?.value().replace(this.replaceRegex, "");
  }
  registerOnChange(fn: (value: string) => void): void {
      this.onChange = fn;
  }
  registerOnTouched(fn: () => void): void {
      this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
      this.disabled = isDisabled;
  }

  onBlur(): void {
    this.onTouched();
  }

  onChangeEvent(event: string): void {
    this.onChange(event);
  }

  private readonly replaceRegex = /^[\u200B\u200C\u200D\u200E\u200F\uFEFF]/;
  private easyMdeInstance: EasyMDE | null = null;

  @Input() easyMdeConfig: Omit<Options, "element"> | undefined;
  @Output() onMarkdownContentChange = new EventEmitter<string>();

  @ViewChild("easymde", { read: ElementRef, static: true })
  readonly easymdeContainer: ElementRef<HTMLElement> | undefined;

  ngAfterViewInit(): void {
    this.draw();
  }

  ngOnDestroy(): void {
    this.destroyEasyMde();
  }

  private draw() {
    this.destroyEasyMde();
    if (this.easymdeContainer && this.easyMdeInstance === null) {
      const defaltOpts: Options = {
        toolbar: [
          "bold",
          "italic",
          "strikethrough",
          "|",
          "heading-1",
          "heading-2",
          "heading-3",
          "|",
          "code",
          "quote",
          "unordered-list",
          "ordered-list",
          "|",
          "link",
          "undo",
          "redo",
          "|",
          "fullscreen",
          "guide"
        ],
        element: this.easymdeContainer.nativeElement,
        spellChecker: false,
        autofocus: true,
        tabSize: 2
      };

      this.easyMdeInstance = new EasyMDE(
        this.easyMdeConfig
          ? { ...this.easyMdeConfig, element: this.easymdeContainer.nativeElement }
          : defaltOpts
      );

      this.easyMdeInstance.codemirror.on("change", () => {
        this.onMarkdownContentChange.emit(
          this.easyMdeInstance?.value().replace(this.replaceRegex, "")
        );
        this.onChangeEvent(this.easyMdeInstance.value())
      });

      this.easyMdeInstance.codemirror.on("blur", () => {
        this.onBlur();
      })

    }
  }

  private destroyEasyMde() {
    if (this.easyMdeInstance) {
      this.easyMdeInstance.toTextArea();
      this.easyMdeInstance.cleanup();
      this.easyMdeInstance = null;
    }
  }
}
