import { NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';

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
    console.log(this.registerForm.value);
  }

  cancelForm() {
    this.router.navigate(['/']);
  }
}
