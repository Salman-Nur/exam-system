import { Component, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { absoluteRoutes } from "../../../../shared/others/misc/absolute-route.constants";

@Component({
  selector: "app-already-logged-in",
  standalone: true,
  imports: [RouterLink],
  templateUrl: "./already-logged-in.component.html",
  styleUrl: "./already-logged-in.component.scss"
})
export class AlreadyLoggedInComponent {
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
}
