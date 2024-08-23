import { Component, DestroyRef, inject, Input, OnInit } from "@angular/core";
import {
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  RouteConfigLoadEnd,
  RouteConfigLoadStart,
  Router
} from "@angular/router";
import { NgxSpinnerComponent } from "ngx-spinner";
import { LoadingSpinnerService } from "../others/loading-spinner.service";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-loading-spinner",
  standalone: true,
  imports: [NgxSpinnerComponent],
  templateUrl: "./loading-spinner.component.html",
  styleUrl: "./loading-spinner.component.scss"
})
export class LoadingSpinnerComponent implements OnInit {
  readonly loadingSpinnerService = inject(LoadingSpinnerService);
  private readonly router = inject(Router);
  private readonly ref = inject(DestroyRef);

  @Input()
  detectRoutingOngoing = false;

  ngOnInit() {
    if (this.detectRoutingOngoing) {
      this.router.events.pipe(takeUntilDestroyed(this.ref)).subscribe((event) => {
        if (event instanceof NavigationStart || event instanceof RouteConfigLoadStart) {
          this.loadingSpinnerService.loadingOn();
        } else if (
          event instanceof NavigationEnd ||
          event instanceof NavigationError ||
          event instanceof NavigationCancel ||
          event instanceof RouteConfigLoadEnd
        ) {
          this.loadingSpinnerService.loadingOff();
        }
      });
    }
  }
}
