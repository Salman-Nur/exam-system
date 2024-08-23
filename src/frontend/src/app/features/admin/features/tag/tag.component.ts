import { Component, OnInit, ViewChild } from '@angular/core';
import { TagManagementService } from '../../service/tag/tag-management.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TagListDTO, TagCreateDTO, TagUpdateDTO } from "../../model/tag.model";
import {LoadingSpinnerComponent} from "../../../../shared/loading-spinner/component/loading-spinner.component";
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-tag',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    LoadingSpinnerComponent, NgbPaginationModule,
    ReactiveFormsModule],
  templateUrl: './tag.component.html',
  styleUrls: ['./tag.component.scss']
})

export class TagComponent implements OnInit {
  tagForm: FormGroup;
  tags: TagListDTO[] = [];
  totalTags: number = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize: number = 10;
  pageIndex: number = 1;
  editMode: boolean = false;
  editTagId: string | null = null;
  message: string | null = null;
  messageType: 'success' | 'error' | null = null;

  constructor(private fb: FormBuilder, private tagService: TagManagementService) {
    this.tagForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadTags();
  }

  clearMessage(): void {
    this.message = null;
    this.messageType = null;
  }

  async loadTags() {
    try {
      const response = await firstValueFrom(this.tagService.getTags(this.pageIndex, this.pageSize));
      this.tags = response.data;
      this.totalTags = response.total;
    } catch (error) {
      this.message = 'Failed to load tags.';
      this.messageType = 'error';
      this.tags = [];
      this.totalTags = 0;
    }
  }

  async onSubmit() {
    if (this.tagForm.invalid) return;

    try {
      if (this.editMode) {
        const updatedTag: TagUpdateDTO = this.tagForm.value;
        await firstValueFrom(this.tagService.updateTag(this.editTagId!, updatedTag));
        this.message = 'Tag updated successfully!';
        this.messageType = 'success';
      } else {
        const newTag: TagCreateDTO = this.tagForm.value;
        await this.tagService.addTag(newTag);
        this.message = 'Tag created successfully!';
        this.messageType = 'success';
      }
      await this.loadTags();
      this.resetForm();
    } catch (error: any) {
      if(typeof(error.error) == typeof("")){
        this.message = error.error;
      }
      else{
        this.message = "Field can't be null or whitespace";
      }
      this.messageType = 'error';
    }
  }

  onEdit(tag: TagListDTO) {
    this.editMode = true;
    this.tagForm.patchValue({ name: tag.name });
    this.editTagId = tag.id;
  }

  async onDelete(id: string) {
    try {
      await firstValueFrom(this.tagService.deleteTag(id));
      this.message = 'Tag deleted successfully!';
      this.messageType = 'success';
      this.loadTags();
    } catch (error) {
      this.message = 'Failed to delete tag.';
      this.messageType = 'error';
    }
  }

  resetForm() {
    this.tagForm.reset();
    this.editMode = false;
    this.editTagId = null;
  }

  onPageChange(page: number) {
    this.pageIndex = page;
    this.loadTags();
  }

  onPageSizeChange(size: number) {
    this.pageSize = size;
    this.pageIndex = 1;
    this.loadTags();
  }
}
