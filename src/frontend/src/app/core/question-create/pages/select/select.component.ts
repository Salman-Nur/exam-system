import { Component, ElementRef, forwardRef, inject, Input, OnInit, ViewChild } from "@angular/core";
import { NgSelectModule } from '@ng-select/ng-select';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { Tag } from "../create/question-create.component";
import { TagManagementService } from "../../../../features/admin/service/tag/tag-management.service";
import { TagCreateDTO } from "../../../../features/admin/model/tag.model";

@Component({
  selector: 'app-select',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, NgSelectModule],
  templateUrl: './select.component.html',
  styleUrl: './select.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    }
  ]
})
export class SelectComponent implements ControlValueAccessor, OnInit
{
  @ViewChild('selectTags') selectTagsElement: ElementRef;
  @Input() tagOptions: Tag[] = [];
  selectedOptions:string[] = [];

  private onValueChange: (value: string[]) => void = (tag: string[]) => {};
  onTouched: any = () => {};
  tagService = inject(TagManagementService);

  async ngOnInit(): Promise<void>
  {
    this.tagOptions = await this.tagService.getAllTags();
  }

  loading = false;

  async onAdd(tagId: { label: string}): Promise<void>{

    if(tagId.label !== undefined) {
      try {
        this.loading = true;
        const tagCreateDto : TagCreateDTO = {
          name: tagId.label
        };
        const id = await this.tagService.addTag(tagCreateDto);
        this.selectedOptions.push(id);
        this.tagOptions.push({id: id, name: tagId.label});
        this.loading = false;
        this.onChange();
      }catch (err){
        this.loading = false;
      }
    }else{
      const id:any = tagId;
      this.selectedOptions.push(id.toString());
      this.onChange();
      return;
    }
  }

  onRemove(tagId: string): void{
    const tag = this.tagOptions.find((tag) => tag.name === tagId);
    if(tag){
      this.selectedOptions = this.selectedOptions.filter((t) => t !== tag.id);
      this.onChange();
      return;
    }
    this.selectedOptions = this.selectedOptions.filter((t) => t !== tagId);
    console.log(this.selectedOptions);
    this.onChange();
  }

  writeValue(value: any): void
  {
    if (value) {
      this.selectedOptions = value;
    }
  }

  registerOnChange(fn: (value:string[]) => void): void
  {
    this.onValueChange = fn;
  }

  registerOnTouched(fn: () => void): void
  {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void{}

  onChange() {
    this.onValueChange(this.selectedOptions);
    this.onTouched();
  }
}
