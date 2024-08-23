import { HttpClient, HttpErrorResponse, HttpStatusCode, HttpHeaders } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { environment } from "../../../environments/environment.development";
import { Member } from "./member.model";
import { firstValueFrom, Observable} from "rxjs";
import { PlainErrorResponse } from "../../core/auth/models/http-responses.model";

@Injectable({
  providedIn: "root"
})
export class MemberService {
  private readonly httpClient = inject(HttpClient);

  Get(): Observable<string | null> {
    return this.httpClient.get<string | null>(`${environment.API_BASE_URL.V1}Member/`);
  }

  async EditMeMber(dto: Member): Promise<"ok" | PlainErrorResponse> {
    try {
      const formData = new FormData();
      formData.append('Id', dto.Id??'');
      formData.append('FullName', dto.FullName??'');
      formData.append('Email', dto.Email??'');
      if (dto.ProfilePicture) {
        formData.append('ProfilePicture', dto.ProfilePicture);
      }

      const headers = new HttpHeaders();
      headers.append('Content-Type', 'multipart/form-data');

      const res = await firstValueFrom(
        this.httpClient.put(environment.API_BASE_URL.V1.concat('Member/'), formData, {
          observe: "response",
          headers: headers
        })
      );

      if (res.ok) {
        return "ok";
      }
      return new PlainErrorResponse("Something went wrong");
    } catch (errResponse) {
      if (errResponse instanceof HttpErrorResponse) {
        if (
          errResponse.status === HttpStatusCode.Unauthorized ||
          errResponse.status === HttpStatusCode.NotFound
        ) {
          return new PlainErrorResponse("Invalid attempt");
        }
      }
      return new PlainErrorResponse("Something went wrong");
    }
  }
}
