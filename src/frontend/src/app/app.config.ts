import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
  withRouterConfig
} from "@angular/router";
import { provideToastr } from "ngx-toastr";
import { provideAnimations } from "@angular/platform-browser/animations";
import { routes } from "./app.routes";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { authInterceptor } from "./core/auth/others/auth.interceptor";
import { RECAPTCHA_V3_SITE_KEY } from "ng-recaptcha-2";
import { environment } from "../environments/environment";
import { loadingSpinnerInterceptor } from "./shared/loading-spinner/others/loading-spinner.interceptor";
import { provideMarkdown } from "ngx-markdown";

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: RECAPTCHA_V3_SITE_KEY,
      useValue: environment.RECAPTCHA_V3_SITE_KEY
    },
    provideMarkdown(),
    provideAnimations(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideHttpClient(withInterceptors([loadingSpinnerInterceptor, authInterceptor])),
    provideRouter(
      routes,
      withComponentInputBinding(),
      withInMemoryScrolling({ scrollPositionRestoration: "enabled" }),
      withRouterConfig({ paramsInheritanceStrategy: "always" })
    ),
    provideToastr({
      maxOpened: 1,
      autoDismiss: true,
      preventDuplicates: true,
      includeTitleDuplicates: true,
      newestOnTop: true,
      progressBar: true,
      timeOut: 3500,
      closeButton: true,
      resetTimeoutOnDuplicate: true,
      positionClass: "toast-bottom-right"
    })
  ]
};
