import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7294/api/Course';

  getCourses(page: number, pageSize: number, keyword: string = '', sort: string = '') {
    return this.http.get<any>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}&keyword=${encodeURIComponent(keyword)}&sort=${sort}`);
  }

  addCourse(data: any) { 
    return this.http.post(this.apiUrl, data); 
  }

  updateCourse(id: number, data: any) {
    return this.http.put(`${this.apiUrl}/${id}`, data); 
  }

  deleteCourse(id: number) { 
    return this.http.delete(`${this.apiUrl}/${id}`); 
  }

  importCoursesFromExcel(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.apiUrl}/import`, formData);
  }

  downloadTemplate() { 
    return this.http.get(`${this.apiUrl}/template`, { responseType: 'blob' }); 
  }

  uploadCurriculum(id: number, file: File) {
  const formData = new FormData();
  formData.append('file', file);
  return this.http.post<{url: string}>(`${this.apiUrl}/${id}/curriculum`, formData);
  }  
}