import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { UserDto } from '../_models/userDto';
import { UserService } from '../_services/user.service';
import { take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf, RouterLink],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit{
  private authService = inject(AuthService)
  private userService = inject(UserService)
  private router = inject(Router)
  private toastr = inject(ToastrService)
  user: UserDto | null = null;
  isAdmin: boolean | null = null;

  constructor() {
    
  }

  ngOnInit(): void {
    this.userService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })

    this.isAdmin = this.authService.getRoleFromToken() === 'Admin' ? true : false;
    
    this.loadUserProfile();
  }

  loadUserProfile() {
    if (!this.user)
      this.userService.getProfile().subscribe({
        next: (user: UserDto) => {
          this.user = user;
        },
        error: (error) => {
          const apiMessage = error.error?.message || ['Failed to load user profile'];
          this.toastr.error(apiMessage, 'Error')
        }
    })
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
