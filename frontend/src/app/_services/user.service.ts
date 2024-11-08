import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateDto } from '../_models/createDto';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { UserDto } from '../_models/userDto';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:7296/api/';
  private currUserSource = new BehaviorSubject<UserDto | null>(null);
  currentUser$ = this.currUserSource.asObservable();


  register(model: any) {
    return this.http.post<CreateDto>(this.baseUrl + 'users', model)
  }

  getProfile(): Observable<UserDto> {
    const token = localStorage.getItem('token');
    if (!token) throw new Error("Token not found");

    const headers = this.getAuthHeaders(token);
    return this.http.get<UserDto>(this.baseUrl + 'profile', {headers}).pipe(
      tap((user : UserDto) => {
        if (user) {
          this.setUser(user);
          this.currUserSource.next(user);
        }
      })
    )
  }

  private setUser(user: UserDto) {
    localStorage.setItem('user', JSON.stringify(user));
  }

  private getAuthHeaders(token: string): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  clearUser() {
    localStorage.removeItem('user');
    this.currUserSource.next(null);
  }
}
