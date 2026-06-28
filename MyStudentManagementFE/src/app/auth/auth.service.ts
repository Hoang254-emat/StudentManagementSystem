import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7294/api/Auth';

  // Đăng nhập hệ thống
  login(credentials: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response && response.accessToken) {
          localStorage.setItem('token', response.accessToken); 
        }
      })
    );
  }

  // Đăng ký tài khoản Sinh viên (Cần truyền ID trung tâm cấp trong data)
  registerStudent(data: any) {
    return this.http.post(`${this.apiUrl}/register/student`, data, { responseType: 'text' });
  }

  // Đăng ký tài khoản Giáo viên (Cần truyền ID trung tâm cấp trong data)
  registerTeacher(data: any) {
    return this.http.post(`${this.apiUrl}/register/teacher`, data, { responseType: 'text' });
  }

  // Đăng xuất
  logout() {
    localStorage.removeItem('token');
  }

  // Lấy token 
  getToken() {
    return localStorage.getItem('token');
  }
}