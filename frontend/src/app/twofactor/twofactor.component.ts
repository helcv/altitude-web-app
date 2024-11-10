import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { TokenDto } from '../_models/tokenDto';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { environment } from '../../environments/environment';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-twofactor',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './twofactor.component.html',
  styleUrl: './twofactor.component.css'
})
export class TwofactorComponent implements OnInit {
  fb = inject(FormBuilder)
  authService = inject(AuthService)
  toastr = inject(ToastrService)
  router = inject(Router)
  route = inject(ActivatedRoute)
  twoFactorForm: FormGroup;
  email: string | null = null;
  provider = environment.provider;


  constructor() {
    this.twoFactorForm = this.fb.group({
      token: ['', [Validators.required, Validators.minLength(6)]],
    });
  }
  
  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'];
    });
  }

  twoFactor() {
    const twoFactor = { email: this.email,
        provider: this.provider,
        token: this.twoFactorForm.value.token
     };

    this.authService.twoFactorAuth(twoFactor).subscribe({
      next: (response: TokenDto) => {
        if(response.token){
          this.toastr.success('Authentication successful', 'Success')
          this.router.navigate(['/profile'])
        }
        else {
          this.toastr.error('Please enter a valid code', 'Error')
        }       
      },
      error: (error) => {
        const apiMessage = error.error?.message || ['Two auth failed'];
        this.toastr.error(apiMessage, 'Error')
      }
    })
  }
}
