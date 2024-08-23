import { HttpInterceptorFn } from "@angular/common/http";
import { SkipLoadingSpinner } from "./skip-loading-spinner.context-token";
import { inject } from "@angular/core";
import { LoadingSpinnerService } from "./loading-spinner.service";
import { finalize } from "rxjs";

export const loadingSpinnerInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.context.get(SkipLoadingSpinner)) {
    return next(req);
  }

  const loadingSpinnerService = inject(LoadingSpinnerService);
  loadingSpinnerService.loadingOn();

  return next(req).pipe(
    finalize(() => {
      loadingSpinnerService.loadingOff();
    })
  );
};
