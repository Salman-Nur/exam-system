import { Component, EventEmitter, inject, Input, Output, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { QuestionPreviewModel } from '../create/question-create.component';
import { ButtonDirective } from '../../directive/button.directive';
import { MarkdownComponent } from "ngx-markdown";

@Component({
  selector: 'app-preview',
  standalone: true,
  imports: [ButtonDirective, MarkdownComponent],
  templateUrl: './preview.component.html',
  styleUrl: './preview.component.scss'
})
export class PreviewComponent
{

  @Input() previewValue: QuestionPreviewModel;
  @Output() notifyCaller: EventEmitter<void> = new EventEmitter<void>();
  @Input() disabled: boolean = true;

  private modalService = inject(NgbModal);
  openLg(content: TemplateRef<any>)
  {
    this.modalService.open(content, { size: 'lg' });
  }

  save()
  {
    this.notifyCaller.emit();
  }

}
