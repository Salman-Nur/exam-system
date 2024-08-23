import { NotFoundComponent } from "../../../core/error/pages/not-found/not-found.component";
import { Routes } from "@angular/router";

export const sharedFallbackRoutes: Routes = [
  {
    path: "**",
    component: NotFoundComponent
  }
];
