import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CreateDto } from '../_models/createDto';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:7296/api/';

  register(model: any) {
    return this.http.post<CreateDto>(this.baseUrl + 'users', model)
  }
}
