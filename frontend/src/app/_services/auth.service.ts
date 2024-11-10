import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, model } from '@angular/core';
import { map } from 'rxjs';
import { TokenDto } from '../_models/tokenDto';
import { UserService } from './user.service';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../environments/environment';
import { GoogleSignInDto } from '../_models/googleSignInDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private userService = inject(UserService)
  baseUrl = environment.apiUrl;

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

  googleSignIn(model: any) {
    return this.http.post<GoogleSignInDto>(this.baseUrl + 'auth/signin-google', model).pipe(
      map((response: GoogleSignInDto) => {
        const googleSignInDto = response;
        if (googleSignInDto.token){
          this.setToken(googleSignInDto.token)
        }
      })
    )
  }

  confirmEmail(token: string, email: string) {
    const params = new HttpParams()
      .set('token', token)
      .set('email', email);
  
    return this.http.get(`${this.baseUrl}auth/emailconfirmation`, { params });
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
