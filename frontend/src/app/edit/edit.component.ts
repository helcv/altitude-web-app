import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';
import { NgIf } from '@angular/common';
import { UserDto } from '../_models/userDto';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-edit',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './edit.component.html',
  styleUrl: './edit.component.css'
})
export class EditComponent implements OnInit {
  editDetailsForm: FormGroup;
  changePasswordForm: FormGroup;
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private userService = inject(UserService);
  private authService = inject(AuthService)
  private toastr = inject(ToastrService);
  user: UserDto | null = null;
  maxDate: string | null = null;
  isAdmin: boolean | null = null;

  constructor() {
    this.setDate()

    this.editDetailsForm = this.fb.group({
      name: ['',],
      lastName: ['',],
      dateOfBirth: ['',],
      profilePhoto: [null],
      twoFactorEnabled: []
    })

    this.changePasswordForm = this.fb.group({
      oldPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(7), CustomValidators.passwordStrength]]
    })
  }

  ngOnInit(): void {
    this.isAdmin = this.authService.getRoleFromToken() === 'Admin' ? true : false;

    this.userService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        this.user = user;
        this.updateForm(this.user); 
      }
    })
    this.loadUserProfile();
    console.log(this.user?.isAuthWithGoogle)
  }

  editProfile() {
    if (this.editDetailsForm.valid) {
      this.userService.editProfileDetails(this.editDetailsForm.value).subscribe({
        next: () => {
          this.loadUserProfile();
          this.toastr.success('Profile updated successfully.');
        },
        error: (error) => {
          const apiMessage = error.error?.message || ['Failed to update user profile'];
          this.toastr.error(apiMessage, 'Error')
        }
      });
    }
  }

  toggleTwoFactor() {
    const isEnabled = this.editDetailsForm.get('twoFactorEnabled')?.value;
    const twoFactor = { IsEnabled: isEnabled };

    this.userService.enableTwoFactor(twoFactor).subscribe(
      () => {
        this.toastr.success(
          `Two-factor authentication ${isEnabled ? 'enabled' : 'disabled'}.`
        );
      },
      (error) => {
        this.toastr.error('Failed to update two-factor authentication.');
        this.editDetailsForm.get('twoFactorEnabled')?.setValue(!isEnabled, {
          emitEvent: false,
        });
      }
    );
  }

  changePassword() {
    console.log(this.changePasswordForm.value);
    if (this.changePasswordForm.valid) {
      this.userService.changePassword(this.changePasswordForm.value).subscribe({
        next: () => {
          this.changePasswordForm.reset();
          this.toastr.success('Password changed successfully.');
          this.authService.logout()
          this.router.navigateByUrl('');
        },
        error: (error) => {
          const apiMessage = error.error?.message || ['Failed to update user password'];
          this.toastr.error(apiMessage, 'Error')
        }
      });
    }
  }

  redirectToProfile() {
    this.router.navigate(['/profile']);
  }

  loadUserProfile() {
    this.userService.getProfile().subscribe({
      next: (user: UserDto) => {
        console.log(user.name)
        this.user = user;
        this.updateForm(this.user);
      },
      error: (error) => {
        const apiMessage = error.error?.message || ['Registration failed'];
      this.toastr.error(apiMessage, 'Error')
      }
    })
  }

  private updateForm(user: UserDto | null): void {
    console.log(user)
    if (user && this.editDetailsForm) {
      this.editDetailsForm.patchValue({
        name: user.name,
        lastName: user.lastName,
        dateOfBirth: user.dateOfBirth,
        twoFactorEnabled: user.isTwoFactorEnabled
      });
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
        if (file.type.startsWith('image/')) {
            this.editDetailsForm.patchValue({
                profilePhoto: file
            });
        } else {
            alert('Please select a valid image file.');
            event.target.value = '';
        }
    }
  }

  private setDate() {
    const today = new Date();
    const maxDate = new Date();
    maxDate.setFullYear(today.getFullYear() - 15);
    this.maxDate = maxDate.toISOString().split('T')[0];
  }
}
