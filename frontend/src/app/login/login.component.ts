import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators } from '../_validators/custom-validators';
import { NgIf } from '@angular/common';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  loginForm : FormGroup;
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService)
  private toastr = inject(ToastrService)

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    })
  }

  ngOnInit(): void {
    
  }

  login() {
    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.router.navigate(['/profile']);
      },
      error: (error) => {
        const apiMessage = error.error?.message || ['Registration failed'];
        this.toastr.error(apiMessage, 'Error')
      }
    })
  }

  cancelForm() {
    this.router.navigate(['/']);
  }
}
