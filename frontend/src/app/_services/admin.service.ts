import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from '../_models/userDto';
import { environment } from '../../environments/environment';
import { UserQueryParams } from '../_models/queryParams';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient)

  constructor() { }

  getUsers(queryParams: UserQueryParams): Observable<UserDto[]> {
    const token = localStorage.getItem('token');
    if (!token) throw new Error("Token not found");
  
    const headers = this.getAuthHeaders(token);
  
    let queryParamsString = '';
    if (queryParams.searchTerm) {
      queryParamsString += `search=${queryParams.searchTerm}`;
    }
    if (queryParams.startDate) {
      queryParamsString += queryParamsString ? `&startDate=${queryParams.startDate}` : `startDate=${queryParams.startDate}`;
    }
    if (queryParams.endDate) {
      queryParamsString += queryParamsString ? `&endDate=${queryParams.endDate}` : `endDate=${queryParams.endDate}`;
    }
    if (queryParams.isVerified !== null) {
      queryParamsString += queryParamsString ? `&isverified=${queryParams.isVerified}` : `&isverified=${queryParams.isVerified}`;
    }
  
    const query = queryParamsString ? `?${queryParamsString}` : '';
    return this.http.get<UserDto[]>(`${this.baseUrl}users${query}`, { headers });
  }

  deleteUser(id: string): Observable<string> {
    const token = localStorage.getItem('token');
    if (!token) throw new Error("Token not found");

    const headers = this.getAuthHeaders(token);
    return this.http.delete<string>(`${this.baseUrl}users/${id}`, { headers });
  }

  private getAuthHeaders(token: string): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
}
