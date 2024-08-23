import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom} from "rxjs";
import { Observable,of  } from 'rxjs';
import { TagListDTO, TagCreateDTO, TagUpdateDTO } from "../../model/tag.model";
import { environment } from "../../../../../environments/environment.development";
import { Tag } from "../../../../core/question-create/pages/create/question-create.component";


@Injectable({
  providedIn: 'root'
})

export class TagManagementService {
  constructor(private http: HttpClient) {}

  getTags(page: number, pageSize: number): Observable<{ data: TagListDTO[], total: number, totalDisplay: number }> {
    const params = new HttpParams()
      .set('pageIndex', page)
      .set('pageSize', pageSize);

    return this.http.get<{ data: TagListDTO[], total: number, totalDisplay: number }>(`${environment.API_BASE_URL.V1}tag`, { params });
  }

  addTag(tag: TagCreateDTO): Promise<string> {
    return firstValueFrom(this.http.post<string>(`${environment.API_BASE_URL.V1}tag`, tag));
  }

  updateTag(id: string, tag: TagUpdateDTO): Observable<void> {
    return this.http.put<void>(`${environment.API_BASE_URL.V1}tag/${id}`, tag);
  }

  deleteTag(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.API_BASE_URL.V1}tag/${id}`);
  }

  getAllTags(): Promise<Tag[]> {
    return firstValueFrom(this.http.get<Tag[]>(environment.V1.Tags));
  }
}
