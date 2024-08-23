import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AccountService } from '../../auth/services/account.service';
import { environment } from '../../../../environments/environment';
import { HttpSuccessOutcome, PlainErrorResponse } from '../../auth/models/http-responses.model';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {

  private readonly httpClient = inject(HttpClient);
  private readonly accountService = inject(AccountService);

  public async createQuestion(formData: FormData): Promise<HttpSuccessOutcome.Created | PlainErrorResponse> {
    try{
      await this.accountService.loadXsrfToken(false);

      const res =  await firstValueFrom(
        this.httpClient.post(environment.V1.QUESTION, formData, { observe: "response" }));

      if(res.status === 201){
        return HttpSuccessOutcome.Created;
      }else{
        return new PlainErrorResponse("Something went wrong");
      }
    }
    catch(err){
      return new PlainErrorResponse("Something went wrong");
    }
  }

}
