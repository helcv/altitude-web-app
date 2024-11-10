import { Component, inject, OnInit } from '@angular/core';
import { UserDto } from '../_models/userDto';
import { AdminService } from '../_services/admin.service';
import { ToastrService } from 'ngx-toastr';
import { NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UserQueryParams } from '../_models/queryParams';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [NgFor, FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  users: UserDto[] = []
  private adminService = inject(AdminService)
  private toastr = inject(ToastrService)
  private router = inject(Router)
  queryParams: UserQueryParams = new UserQueryParams();
  ngOnInit(): void {
    this.loadUsers()
  }


  loadUsers() {
    this.adminService.getUsers(this.queryParams).subscribe({
      next: (users: UserDto[]) => {
        this.users = users
      },
      error: () => {
        this.toastr.error('Error fetching users')
      }
    })
  }

  onSearch() {
    this.loadUsers()
  }

  deleteUser(id: string) {
    this.adminService.deleteUser(id).subscribe({
      next: (response) => {
        this.toastr.success('User successfully deleted', 'Success');
        this.loadUsers();
      },
      error: (error) => {
        const apiMessage = error.error?.message || 'Failed to delete user profile';
        this.toastr.error(apiMessage, 'Error');
      }
    });
  }

  redirectToProfile() {
    this.router.navigate(['/profile']);
  }
}
