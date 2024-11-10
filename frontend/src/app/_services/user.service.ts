import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, model } from '@angular/core';
import { CreateDto } from '../_models/createDto';
import { BehaviorSubject, Observable, switchMap, tap } from 'rxjs';
import { UserDto } from '../_models/userDto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  private currUserSource = new BehaviorSubject<UserDto | null>(null);
  currentUser$ = this.currUserSource.asObservable();


  register(model: any) {
    return this.http.post<CreateDto>(this.baseUrl + 'users', model)
  }

  editProfileDetails(model: any) {
    const formData = new FormData();
    for (const key in model) {
        if (model.hasOwnProperty(key)) {
            formData.append(key, model[key]);
        }
    }

    return this.http.put<string>(this.baseUrl + 'profile/details', formData)
   }

  changePassword(model: any) {
    return this.http.put<string>(this.baseUrl + 'profile/password', model)
   }

   enableTwoFactor(model: any) {
    return this.http.put(this.baseUrl + 'auth/twofactor', model)
   }

  getProfile(): Observable<UserDto> {
    return this.http.get<UserDto>(this.baseUrl + 'profile').pipe(
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

  clearUser() {
    localStorage.removeItem('user');
    this.currUserSource.next(null);
  }
}
