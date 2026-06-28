import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7294/api/Student';

  getStudents(page: number, pageSize: number, keyword: string = '', sort: string = '') {
    let queryUrl = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    
    if (keyword) {
      queryUrl += `&keyword=${encodeURIComponent(keyword)}`;
    }
    if (sort) {
      queryUrl += `&sort=${sort}`;
    }
    
    return this.http.get<any>(queryUrl);
  }

  addStudent(student: any){
    return this.http.post<any>(this.apiUrl, student);
  }

  importStudentsFromExcel(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<any>(`${this.apiUrl}/import`, formData);
  }

  updateStudent(id: string, student: any) {
    return this.http.put(`${this.apiUrl}/${id}`, student);
  }

  deleteStudent(id: string){
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  downloadTemplate() {
  return this.http.get(`${this.apiUrl}/template`, { responseType: 'blob' }); 
  }

  getMyProfile() {
    return this.http.get<any>(`${this.apiUrl}/my-profile`);
  }

  uploadAvatar(id: string, file: File) {
    const formData = new FormData();
    formData.append('file', file); 

    return this.http.post<any>(`${this.apiUrl}/${id}/avatar`, formData);
  }
}
