import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ClassService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7294/api/Class';

  getClasses(page: number, pageSize: number, keyword: string = '', sort: string = '') {
    let queryUrl = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    
    if (keyword) {
      queryUrl += `&keyword=${encodeURIComponent(keyword)}`;
    }
    if (sort) {
      queryUrl += `&sort=${sort}`;
    }
    
    return this.http.get<any>(queryUrl);
  }

  addClass(data: any) { 
    return this.http.post<any>(this.apiUrl, data); 
  }

  updateClass(id: number, data: any) { 
    return this.http.put(`${this.apiUrl}/${id}`, data); 
  }

  deleteClass(id: number) { 
    return this.http.delete<any>(`${this.apiUrl}/${id}`); 
  }

  importClassesFromExcel(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.apiUrl}/import`, formData);
  }

  downloadTemplate() {
    return this.http.get(`${this.apiUrl}/template`, { responseType: 'blob' }); 
  }
}