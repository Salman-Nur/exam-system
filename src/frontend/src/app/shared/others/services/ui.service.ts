import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class UiService {
  private readonly sidenavOpen = new BehaviorSubject(false);
  readonly isSideNavOpen$ = this.sidenavOpen.asObservable();

  openSideNav() {
    this.sidenavOpen.next(true);
  }

  closeSideNav() {
    this.sidenavOpen.next(false);
  }
}
