import { HttpErrorResponse, HttpInterceptorFn, HttpStatusCode } from "@angular/common/http";
import { XSRF_TOKEN } from "./auth.constants";
import { tap } from "rxjs";
import { inject } from "@angular/core";
import { ToastrService } from "ngx-toastr";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const toastrService = inject(ToastrService);
  const xsrfToken = sessionStorage.getItem(XSRF_TOKEN);
  const clonedReq = req.clone({
    withCredentials: true,
    setHeaders: xsrfToken ? { "X-XSRF-TOKEN": xsrfToken } : {}
  });

  return next(clonedReq).pipe(
    tap({
      error: (event) => {
        if (event instanceof HttpErrorResponse) {
          if (event.status === HttpStatusCode.Forbidden && event.error == "xsrf error") {
            toastrService.error(
              "Try closing and reopening your browser",
              "Problem with XSRF token"
            );
          }
        }
      }
    })
  );
};
