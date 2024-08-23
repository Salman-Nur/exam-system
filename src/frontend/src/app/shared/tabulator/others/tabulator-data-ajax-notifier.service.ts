import { Injectable, signal } from "@angular/core";

@Injectable({
  providedIn: "root"
})
export class TabulatorDataAjaxNotifierService {
  private readonly $ongoing = signal<boolean>(false);
  public readonly $$isTabulatorRelatedAjax = this.$ongoing.asReadonly();

  notifyStart() {
    this.$ongoing.set(true);
  }

  notifyStop() {
    this.$ongoing.set(false);
  }
}
