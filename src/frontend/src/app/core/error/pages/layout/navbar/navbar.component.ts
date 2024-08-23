import { Component, inject, OnInit, signal } from '@angular/core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterModule } from '@angular/router';
import { absoluteRoutes } from '../../../../../shared/others/misc/absolute-route.constants';
import { AccountService } from '../../../../auth/services/account.service';
import { AsyncPipe, NgIf } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    NgbDropdownModule,
    RouterModule,
    NgIf,
    AsyncPipe
  ],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})

export class NavbarComponent implements OnInit {
  username: string|null = '';
  avatarimg: string|null= '';
  readonly $$absoluteRoutes = signal(absoluteRoutes).asReadonly();
  readonly router = inject(Router);
  accountService = inject(AccountService);

  constructor() {}

  ngOnInit() {
      let user = this.accountService.$$currentUser();
      console.log("user:" + user);
      if (user) {
        this.username = user.email;
        this.avatarimg = user.profilePicture;
      }
  }

  EditProfile(){
     return this.router.navigateByUrl("member/profile-update");
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl(absoluteRoutes.LOGIN);
  }
}
