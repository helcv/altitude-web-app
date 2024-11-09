import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map } from 'rxjs';
import { TokenDto } from '../_models/tokenDto';
import { UserService } from './user.service';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private userService = inject(UserService)
  baseUrl = 'https://localhost:7296/api/';

  constructor() { }

  login(model: any){
    return this.http.post<TokenDto>(this.baseUrl + 'auth', model).pipe(
      map((response: TokenDto) => {
        const tokenDto = response;
        if (tokenDto) {
          this.setToken(tokenDto.token);
        }
      })
    );
  }

  setToken (token : string){
    localStorage.setItem('token', token);
  }

  getRoleFromToken(): string | null {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      return decodedToken.role;
    }
    return null;
  }

  logout(){
    localStorage.removeItem('token');
    this.userService.clearUser();
  }
}
