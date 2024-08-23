import { Component, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";

@Component({
  selector: "app-not-found",
  standalone: true,
  imports: [RouterLink],
  templateUrl: "./not-found.component.html",
  styleUrl: "./not-found.component.scss"
})
export class NotFoundComponent {
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
}
