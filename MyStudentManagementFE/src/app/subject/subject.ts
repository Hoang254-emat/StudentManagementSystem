import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SubjectService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7294/api/Subject';

  getSubjects(page: number, pageSize: number, keyword: string = '', sort: string = '') {
    let queryUrl = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    
    if (keyword) {
      queryUrl += `&keyword=${encodeURIComponent(keyword)}`;
    }
    if (sort) {
      queryUrl += `&sort=${sort}`;
    }
    
    return this.http.get<any>(queryUrl);
  }

  addSubject(subject: any){
    return this.http.post<any>(this.apiUrl, subject);
  }

  importSubjectsFromExcel(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<any>(`${this.apiUrl}/import`, formData);
  }

  updateSubject(id: string, subject: any) {
    return this.http.put(`${this.apiUrl}/${id}`, subject);
  }

  deleteSubject(id: string){
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  downloadTemplate() {
  return this.http.get(`${this.apiUrl}/template`, { responseType: 'blob' }); 
  }
}
