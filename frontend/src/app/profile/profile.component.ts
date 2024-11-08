import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router } from '@angular/router';
import { UserDto } from '../_models/userDto';
import { UserService } from '../_services/user.service';
import { take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit{
  authService = inject(AuthService)
  userService = inject(UserService)
  router = inject(Router)
  toastr = inject(ToastrService)
  user: UserDto | null = null;

  constructor() {
    this.userService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    if (!this.user)
      this.userService.getProfile().subscribe({
        next: (user: UserDto) => {
          this.user = user;
        },
        error: (error) => {
          const apiMessage = error.error?.message || ['Registration failed'];
        this.toastr.error(apiMessage, 'Error')
        }
    })
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
