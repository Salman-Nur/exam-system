import { Routes } from "@angular/router";
import { sharedFallbackRoutes } from "../../shared/others/misc/shared-fallback.routes";
import { authGuard } from "../auth/guards/auth.guard";

export const questionRoutes: Routes = [
    {
      path: "",
      canActivate: [authGuard],
      loadComponent: () => import("./pages/question-list/question-list.component").then(m => m.QuestionListComponent),
      title: "Question List",
    },
    {
        path: "create",
        canActivate: [authGuard],
        loadComponent: () => import("./pages/create/question-create.component").then((m) => m.QuestionCreateComponent),
        title: "Create Question"
    },
    ...sharedFallbackRoutes
];
