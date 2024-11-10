import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../_services/user.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators } from '../_validators/custom-validators';
import { NgIf } from '@angular/common';
import { AuthService } from '../_services/auth.service';
import { TokenDto } from '../_models/tokenDto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, RouterLink],
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
      next: (response: TokenDto) => {
        if(response.is2FaRequired){
          const email = this.loginForm.value.email;
          this.router.navigate(['/auth/twofactor'], {
            queryParams: { email }, 
          });
        }
        else
          this.router.navigate(['/profile']);
      },
      error: (error) => {
        const apiMessage = error.error?.message || ['Login failed'];
        this.toastr.error(apiMessage, 'Error')
      }
    })
  }
}
