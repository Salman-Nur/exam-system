import { inject, Injectable, signal } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

@Injectable({
  providedIn: "root"
})
export class LoadingSpinnerService {
  private spinner = inject(NgxSpinnerService);
  private readonly $loading = signal<boolean>(false);
  public readonly $$isLoading = this.$loading.asReadonly();

  loadingOn() {
    this.$loading.set(true);
    this.spinner.show().then(null);
  }

  loadingOff() {
    this.$loading.set(false);
    this.spinner.hide().then(null);
  }
}
