import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { environment } from "../../../environments/environment.development";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root"
})

export class ErrorLogService {
  private readonly httpClient = inject(HttpClient);


  getById(id: number): Observable<{ message: string; levelName: string; time: string }> {
    let params = new HttpParams();
    params.append("id",id);
    return this.httpClient.get<{ message: string; levelName: string; time: string }>(environment.API_BASE_URL.V1.concat('log'),{params});
  }


  delete(id: string) {
    let params = new HttpParams();
    params.append("id",id);
    return this.httpClient.delete(environment.API_BASE_URL.V1.concat('log'),{params});
  }

  deleteMany(id: Array<string>) {
    return this.httpClient.post(environment.API_BASE_URL.V1.concat("log/bulk-delete"), { id });
  }
}
