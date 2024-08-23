import { Component, ElementRef, ViewChild, OnInit, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AvatarSvgComponent } from '../../shared/avatar-svg/avatar-svg.component';
import { MemberService } from './member-service';
import { ReactiveFormUtilsService } from '../../shared/others/services/reactive-form-utils.service';
import { NgxBootstrapInputDirective } from '../../shared/others/directives/ngx-bootstrap-input.directive';
import { absoluteRoutes } from '../../shared/others/misc/absolute-route.constants';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../core/auth/services/account.service';
import { NgIf } from '@angular/common';
import { NavbarComponent } from '../../core/error/pages/layout/navbar/navbar.component';
import { LoadingSpinnerService } from '../../shared/loading-spinner/others/loading-spinner.service';

@Component({
  selector: 'app-profile-update',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    AvatarSvgComponent,
    NgxBootstrapInputDirective,
    RouterModule,
    NgIf,
    NavbarComponent
  ],
  templateUrl: './profile-update.component.html',
  styleUrls: ['./profile-update.component.scss']
})

export class ProfileUpdateComponent implements OnInit {
  showImage: boolean = false;
  showSVG: boolean = true;
  imageSrc: any;
  picture: File | undefined;
  memberService = inject(MemberService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly loadingSpinnerService = inject(LoadingSpinnerService);
  private readonly toastr = inject(ToastrService);
  accountService = inject(AccountService);
  errorMessage: string = '';
  readonly $serverSideErrors = signal<Array<string>>([]);
  readonly router = inject(Router);

  @ViewChild('imagePreview', { static: false }) imagePreview: ElementRef<HTMLImageElement> | undefined;

  readonly profileForm = this.formBuilder.group({
    id: [{ value: '', disabled: true }],
    name: ["", [Validators.required, Validators.minLength(6), Validators.maxLength(256)]],
    email: [{ value: '', disabled: true }],
    image: ['']
  });

  constructor() {
  }

  get id() {
    return this.profileForm.controls.id;
  }

  get image() {
    return this.profileForm.controls.image;
  }

  get name() {
    return this.profileForm.controls.name;
  }

  get email() {
    return this.profileForm.controls.email;
  }

  ngOnInit(): void {
    let user = this.accountService.$$currentUser();
    if (user) {
      this.profileForm.patchValue({
        id: user.id ?? '',
        name: user.name ?? '',
        email: user.email ?? '',
        image: user.profilePicture ?? ''
      });
      this.memberService.Get().subscribe({
        next: (res: any) => {
          console.log(res);
          if (this.imagePreview) {
            this.imageSrc = res.profilePictureUrl;
            this.showImage = true;
            this.showSVG = false;
          }
        },
        error: (err: any) => {
          console.error('Error fetching image URL', err);
        }
      });
    }
  }

  triggerFileInput(event: Event): void {
    event.preventDefault();
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const reader = new FileReader();
      if (file.size > 1048576) {
        input.value = '';
        this.picture = undefined;
        this.showImage = false;
        this.showSVG = true;
        this.errorMessage = 'File size must be less than 1 MB.';
        return;
      }
      const allowedTypes = ['image/jpg', 'image/png', 'image/jpeg'];
      if (!allowedTypes.includes(file.type)) {
        input.value = '';
        this.picture = undefined;
        this.showImage = false;
        this.showSVG = true;
        this.errorMessage = 'Only JPEG,JPG and PNG files are allowed.';
        return;
      }
      this.errorMessage = '';
      this.showImage = true;
      this.showSVG = false;
      reader.readAsDataURL(file);
      reader.onload = (e: any) => {
        this.profileForm.patchValue({
          image: e.target.result
        });
        if (this.imagePreview) {
          this.imagePreview.nativeElement.src = e.target.result;
        }
      };
      this.picture = file;
    }
  }

  async onSubmit() {
    if (!this.profileForm.valid) {
      return;
    }

    const res = await this.memberService.EditMeMber({
      Id: this.id.value,
      Email: this.email.value,
      ProfilePicture: this.picture,
      FullName: this.name.value
    });

    if (res === "ok") {
      this.toastr.success("Your Profile Updated", "Success", {
        positionClass: 'toast-bottom-right',
        timeOut: 2000
      });
      await this.router.navigateByUrl(absoluteRoutes.DASHBOARD);
    }
    else {
      {
        this.toastr.error("Something Went Wrong,Try Again Later", "error");
      };
    }
  }
}
