import { Routes } from '@angular/router';
import { ProfileUpdateComponent } from './features/profile-update/profile-update.component';
import { ErrorLogComponent } from './features/error-log/error-log.component';
import { sharedFallbackRoutes } from "./shared/others/misc/shared-fallback.routes";
import { DashboardComponent } from "./features/dashboard/dashboard.component";
import { TagComponent } from "./features/admin/features/tag/tag.component";
import { authGuard } from "./core/auth/guards/auth.guard";

export const routes: Routes = [
  {
    path: "member/profile-update",
    component: ProfileUpdateComponent,
    canActivate: [authGuard],
    title:'Profile Update'
  },
  {
    path: 'admin/error-logs',
    component: ErrorLogComponent,
    canActivate: [authGuard],
  },
  {
    path: 'admin/tag',
    component: TagComponent,
    canActivate: [authGuard],
  },
  {
    path: "",
    redirectTo: "/dashboard",
    pathMatch: "full"
  },
  {
    path: "dashboard",
    canActivate: [authGuard],
    component: DashboardComponent,
    title: "Dashboard"
  },
  {
    path: "account",
    loadChildren: () => import("./core/auth/auth.routes").then((m) => m.authRoutes)
  },
  {
    path: "question",
    loadChildren: () => import("./core/question-create/question.routes").then((m) => m.questionRoutes)
  },
  ...sharedFallbackRoutes
];
