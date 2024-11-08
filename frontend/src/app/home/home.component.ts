import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private router = inject(Router);

  ngOnInit(): void {
    
  }

  openRegistration() {
    this.router.navigate(['/register']);
  }

  openLogIn() {
    this.router.navigate(['/login']);
  }
}
