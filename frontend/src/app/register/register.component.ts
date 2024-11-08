import { NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm : FormGroup;
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private userService = inject(UserService);
  private toastr = inject(ToastrService)

  constructor() {
    this.registerForm = this.fb.group({
      name: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(7), CustomValidators.passwordStrength]]
    })
  }

  ngOnInit(): void {
    
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

  cancelForm() {
    this.router.navigate(['/']);
  }
}
