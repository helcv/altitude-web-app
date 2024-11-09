import { NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { GoogleTokenDto } from '../_models/googleTokenDto';
import { secrets } from '../../environments/environment.secrets';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  googleToken: GoogleTokenDto | null = null;
  googleClientId = secrets.googleClientId;
  maxDate: string | null = null;
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private userService = inject(UserService);
  private authService = inject(AuthService)
  private toastr = inject(ToastrService)

  constructor() {
    this.setDate()

    this.registerForm = this.fb.group({
      name: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(7), CustomValidators.passwordStrength]]
    })
  }

  ngOnInit(): void {
    const googleButtonElement = document.getElementById('google-button') as HTMLElement;
    google.accounts.id.initialize({
      client_id: this.googleClientId,
      callback: (response) => this.handleGoogleSignIn(response)
    });
    google.accounts.id.renderButton(
      googleButtonElement,
      {
        theme: 'outline',
        size: 'large',
        type: 'standard'
      }
    );
  }

  register() {
    if (this.registerForm.valid) {
      this.userService.register(this.registerForm.value).subscribe({
        next: () => {
          this.toastr.success('Registration successful!', 'Success');
          this.router.navigate(['/']);
        },
        error: (error) => {
          const apiMessages = error.error?.messages || ['Registration failed'];
          apiMessages.forEach((message: string) => this.toastr.error(message, 'Error'));
        }
      });
    } else {
      this.toastr.error('Please fill out all required fields correctly.', 'Form Invalid');
    }
  }

  googleSignIn(googleTokenDto: GoogleTokenDto) {
    this.authService.googleSignIn(googleTokenDto).subscribe({
      next: () => {
        this.toastr.success('Google Sign-In successful!', 'Success');
        this.router.navigate(['/profile']);
      },
      error: (error) => {
        const apiMessages = error.error?.messages || ['Google SignIn failed'];
        apiMessages.forEach((message: string) => this.toastr.error(message, 'Error'));
      }
    });
  }

  handleGoogleSignIn(response: any) {
    const googleTokenDto: GoogleTokenDto = { token: response.credential };
    this.googleSignIn(googleTokenDto);
  }

  cancelForm() {
    this.router.navigate(['/']);
  }

  private setDate() {
    const today = new Date();
    const maxDate = new Date();
    maxDate.setFullYear(today.getFullYear() - 15);
    this.maxDate = maxDate.toISOString().split('T')[0];
  }
}
