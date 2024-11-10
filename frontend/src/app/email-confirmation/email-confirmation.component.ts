import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-email-confirmation',
  standalone: true,
  imports: [],
  templateUrl: './email-confirmation.component.html',
  styleUrl: './email-confirmation.component.css'
})
export class EmailConfirmationComponent implements OnInit {
  route = inject(ActivatedRoute)
  authService = inject(AuthService)
  toastr = inject(ToastrService)
  router = inject(Router)

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    const email = this.route.snapshot.queryParamMap.get('email');

    if (token && email) {
      this.authService.confirmEmail(token, email).subscribe(
        () => { 
          setTimeout(() => {
            this.toastr.success('Email confirmed! You can now log in.');
            this.router.navigate(['/']);
          }, 3000);
        },
        (error) => {
          this.toastr.error('Email confirmation failed. Please try again.');
          console.error(error);
        }
      );
    } else {
      this.toastr.error('Invalid confirmation link.');
      this.router.navigate(['/']);
    }
  }
}
