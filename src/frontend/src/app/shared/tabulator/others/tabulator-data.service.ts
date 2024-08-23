import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";

@Injectable()
export class TabulatorDataService {
  // during component init, there will be an ajax request
  // so reloadTable should be false on that point
  private readonly reloadTableSubject = new BehaviorSubject<boolean>(false);
  public readonly canReloadTable$: Observable<boolean> = this.reloadTableSubject.asObservable();

  private readonly ajaxRequestOutgoingSubject = new BehaviorSubject<boolean>(false);
  public readonly isAjaxRequestOutgoing$ = this.ajaxRequestOutgoingSubject.asObservable();

  public reloadTableData() {
    this.reloadTableSubject.next(true);
  }

  public outgoingAjaxRequestInProgress() {
    this.ajaxRequestOutgoingSubject.next(true);
  }

  public outgoingAjaxRequestDone() {
    this.ajaxRequestOutgoingSubject.next(false);
  }
}
