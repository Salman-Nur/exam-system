import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { NavbarComponent } from "./core/error/pages/layout/navbar/navbar.component";
import { LoadingSpinnerComponent } from "./shared/loading-spinner/component/loading-spinner.component";

@Component({
  selector: "app-root",
  standalone: true,
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
  imports: [RouterOutlet, LoadingSpinnerComponent,NavbarComponent]
})
export class AppComponent {
  title : string = '';
}
