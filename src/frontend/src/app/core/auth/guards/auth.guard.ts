import { CanActivateFn, RedirectCommand, Router } from "@angular/router";
import { UiService } from "../../../shared/others/services/ui.service";
import { AccountService } from "../services/account.service";
import { inject } from "@angular/core";
import { absoluteRoutes } from "../../../shared/others/misc/absolute-route.constants";

export const authGuard: CanActivateFn = async (route, state) => {
  const uiService = inject(UiService);
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (await accountService.checkLoggedInFromJwt()) {
    uiService.openSideNav();
    return true;
  } else {
    uiService.closeSideNav();
    return new RedirectCommand(router.parseUrl(absoluteRoutes.LOGIN));
  }
};
