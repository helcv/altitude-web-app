import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserDto } from '../_models/userDto';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = 'https://localhost:7296/api/';
  private http = inject(HttpClient)

  constructor() { }

  getUsers(searchTerm: string = '', startDate: string = '', endDate: string = ''): Observable<UserDto[]> {
    const token = localStorage.getItem('token');
    if (!token) throw new Error("Token not found");
  
    const headers = this.getAuthHeaders(token);
  
    let queryParams = '';
    if (searchTerm) {
      queryParams += `search=${searchTerm}`;
    }
    if (startDate) {
      queryParams += queryParams ? `&startDate=${startDate}` : `startDate=${startDate}`;
    }
    if (endDate) {
      queryParams += queryParams ? `&endDate=${endDate}` : `endDate=${endDate}`;
    }
  
    const query = queryParams ? `?${queryParams}` : '';
    return this.http.get<UserDto[]>(`${this.baseUrl}users${query}`, { headers });
  }

  deleteUser(id: string): Observable<any> {
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
