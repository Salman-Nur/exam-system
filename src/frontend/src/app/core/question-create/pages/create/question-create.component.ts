import { Component, inject, OnInit } from "@angular/core";
import { FormArray, FormControl, NonNullableFormBuilder } from "@angular/forms";
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SelectComponent } from '../select/select.component';
import { PreviewComponent } from '../preview/preview.component';
import { ButtonDirective } from '../../directive/button.directive';
import { QuestionService } from '../../service/question.service';
import { HttpSuccessOutcome } from '../../../auth/models/http-responses.model';
import { MarkdownEditorComponent } from "../../../../shared/markdown-editor/markdown-editor.component";
import { MarkdownComponent } from "ngx-markdown";
import { markValidator } from "./Validators/mark.validator";
import { timeValidator } from "./Validators/time.validator";
import { LoadingSpinnerService } from "../../../../shared/loading-spinner/others/loading-spinner.service";
import { Router } from "@angular/router";
import { difficultyValidator } from "./Validators/difficulty.validator";
import { ToastrService } from "ngx-toastr";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";

export interface Tag{
  id: string;
  name: string;
}

export interface Option
{
  type: MyType;
  isCorrect: boolean;
  value: any;
}

export interface QuestionPreviewModel
{
  title: string;
  descriptions: Array<{ type: string; value: any }>;
  options: Option[];
}

export const MyType = {
  Image: 'image',
  Text: 'text'
} as const;

export type MyType = typeof MyType[keyof typeof MyType];

export type DescriptionItem = {
  type: MyType;
  content: string;
};


@Component({
  selector: 'app-question-create',
  standalone: true,
  templateUrl: './question-create.component.html',
  styleUrl: './question-create.component.scss',
  imports: [
    ReactiveFormsModule,
    SelectComponent,
    PreviewComponent,
    ButtonDirective,
    MarkdownEditorComponent,
    MarkdownComponent
  ]
})
export class QuestionCreateComponent implements OnInit
{
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly questionService = inject(QuestionService);
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToastrService);

  questionCreateForm: FormGroup;
  previews: Array<{type:string, value: any}> = [];
  previewOptions: Option[] = [];
  descriptionValues: Array<{type: string, value:any}> = [];
  optionValues: Option[] = [];
  previewValue: QuestionPreviewModel;

  ngOnInit(): void
  {
    this.questionCreateForm = this.formBuilder.group({
      title: ['', Validators.required],
      inputs: this.formBuilder.array([]),
      options: this.formBuilder.array([]),
      mark: this.formBuilder.control('', [Validators.required, markValidator()]),
      timeLimit: this.formBuilder.control('', [Validators.required, timeValidator()]),
      difficulty: this.formBuilder.control('', [Validators.required, difficultyValidator()]),
      required: this.formBuilder.control(false),
      tags: [[], Validators.required],
    });
  }

  setPreviewValue()
  {
    const title = this.title.value;
    const descriptions = this.previews;
    const options = this.previewOptions;

    if (title && descriptions && options) {
      this.previewValue = { title, descriptions, options };
    }
  }



  hasError(controlName: string, errorName: string) {
    return this.questionCreateForm.get(controlName)?.hasError(errorName);
  }

  get difficulty(){
    return this.questionCreateForm.get('difficulty') as FormControl;
  }

  get mark(){
    return this.questionCreateForm.get('mark') as FormControl;
  }

  get timeLimit(){
    return this.questionCreateForm.get('timeLimit') as FormControl;
  }

  get inputs()
  {
    return this.questionCreateForm.get('inputs') as FormArray;
  }

  get title()
  {
    return this.questionCreateForm.get('title') as FormControl;

  }

  get tagsControl()
  {
    return this.questionCreateForm.get('tags') as FormControl;

  }

  get options()
  {
    return this.questionCreateForm.get('options') as FormArray;
  }

  private async appendToFormData<T>(formData: FormData, type: string, name: string, value: T, filename?:string){
      if(type === MyType.Image){
        formData.append(name, <File>value, filename);
      }else {
        formData.append(name, <string>value);
      }
  }

  private async appendDescriptionToFormData(formData: FormData, descriptionValue: Array<{type: string, value:any}>){
    descriptionValue.forEach((item, index) => {
        const name = `Description-${index}`;
        this.appendToFormData(formData, item.type, name, item.value, `${index}.png`);
    });
  }

  private async appendOptionsToFormData(formData: FormData, options: Option[]){
    options.forEach((item, index) => {
      const name = `Option-${index}`;
      this.appendToFormData(formData, item.type, name, item.value, `${index}.png`);
      this.appendToFormData(formData, MyType.Text, name, item.isCorrect);
    })
  }


  async onSubmit()
  {
    this.loadingSpinnerService.loadingOn();
    const formData: FormData = new FormData();
    formData.append('title', this.title.value);
    formData.append('mark', this.mark.value);
    formData.append('timeLimit', this.timeLimit.value);
    formData.append('required', this.questionCreateForm.value.required);
    formData.append('difficulty', this.difficulty.value);

    await this.appendDescriptionToFormData(formData, this.descriptionValues);
    await this.appendOptionsToFormData(formData, this.optionValues);

    this.tagsControl.value.forEach((id: string) =>
    {
      formData.append('tags', id);
    });

    const res = await this.questionService.createQuestion(formData);
    if (res === HttpSuccessOutcome.Created) {
      this.loadingSpinnerService.loadingOff();
      await this.router.navigateByUrl(absoluteRoutes.QUESTION_LIST);
      this.toaster.success("Question Created successfully.");
    }
    else {
      this.loadingSpinnerService.loadingOff();
      this.toaster.error("Unable to create a question.");
    }
  }

  removeInput(index: number)
  {
    this.inputs.removeAt(index);
    this.previews.splice(index, 1);
    this.descriptionValues.splice(index, 1);
  }

  createDescriptionItem(item: DescriptionItem) : FormGroup{
    return this.formBuilder.group({
      type: [item.type, Validators.required],
      value: [item.content, Validators.required]
    });
  }

  addImage()
  {
    this.inputs.push(this.createDescriptionItem({ type: MyType.Image, content: '' }));
    this.descriptionValues.push({ type: MyType.Image, value: null });
    this.previews.push({ type: MyType.Image, value: null });
  }

  addText()
  {
    this.inputs.push(this.createDescriptionItem({ type: MyType.Text, content: '' }));
    this.descriptionValues.push({ type: MyType.Text, value: '' });
    this.previews.push({ type: MyType.Text, value: '' });
  }

  onImageChange(event: Event, index: number)
  {
    const file = (event.target as HTMLInputElement).files[0];
    this.descriptionValues[index].value = file;

    if (file) {
      const reader = new FileReader();
      reader.onload = () =>
      {
        this.previews[index].value = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onTextChange(value: string, index: number)
  {
    this.descriptionValues[index].value = value;
    this.previews[index].value = value;
  }

  //Options Section

  private createOptionItem(option: Option): FormGroup
  {
    return this.formBuilder.group({
      type: [option.type, Validators.required],
      isCorrect: [option.isCorrect, Validators.required],
      value: [option.value, Validators.required]
    });
  }

  addImageOption()
  {
    this.options.push(this.createOptionItem({ type: MyType.Image, isCorrect: false, value: '' }));
    this.previewOptions.push({ type: MyType.Image, isCorrect: false, value: null });
    this.optionValues.push({ type: MyType.Image, isCorrect: false, value: null });
  }

  addTextOption(){
    this.options.push(this.createOptionItem({ type: MyType.Text, isCorrect: false, value: '' }));
    this.previewOptions.push({ type: MyType.Text, isCorrect: false, value: '' });
    this.optionValues.push({ type: MyType.Text, isCorrect: false, value: '' });
  }

  onOptionTextChange(event: Event, index: number)
  {
    const input = event.target as HTMLInputElement;
    this.previewOptions[index].value = input.value;
    this.optionValues[index].value = input.value;
  }

  onOptionImageChange(event: Event, index: number)
  {
    const file = (event.target as HTMLInputElement).files[0];
    this.optionValues[index].value = file;
    if (file) {
      const reader = new FileReader();
      reader.onload = () =>
      {
        this.previewOptions[index].value = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  removeOption(index: number)
  {
    this.options.removeAt(index);
    this.previewOptions.splice(index, 1);
    this.optionValues.splice(index, 1);
  }

  selectAsCorrect(index: number)
  {
    this.previewOptions.forEach((option, i) =>
    {
      if (i === index) {
        option.isCorrect = !option.isCorrect;
        this.optionValues[i].isCorrect = option.isCorrect;
      }
    });
  }
}
