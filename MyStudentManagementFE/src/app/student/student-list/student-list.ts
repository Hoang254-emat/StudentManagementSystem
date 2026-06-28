import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { StudentService } from '../student';
import { AddStudent } from '../add-student/add-student';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../header/header';

@Component({
  selector: 'app-student-list',
  imports: [AddStudent, DatePipe, FormsModule, HeaderComponent],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss',
  standalone: true,
})
export class StudentList implements OnInit {
  private studentService = inject(StudentService);
  students: any[] = [];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  isFormVisible = false;
  editingStudent: any = null;

  searchInput: string = ''; 

  keyword: string = '';
  sortOption: string = '';

  ngOnInit() {
    this.loadStudents();
  }

  toggleForm() {
    this.isFormVisible = !this.isFormVisible; 
    if (!this.isFormVisible) {
      this.editingStudent = null; 
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      if (confirm(`Bạn có chắc muốn import danh sách từ file ${file.name}?`)) {
        this.studentService.importStudentsFromExcel(file).subscribe({
          next: (res) => {
            alert(res.message);
            this.loadStudents(); 
            event.target.value = ''; 
          },
          error: (err) => {
            alert('Lỗi import: ' + err.error);
            event.target.value = ''; 
          }
        });
      }
    }
  }

  onStudentAdded() {
    this.isFormVisible = false;
    this.editingStudent = null;
    this.currentPage = 1; 
    this.loadStudents(); 
  }

  onSearch() {
    this.keyword = this.searchInput; 
    this.currentPage = 1; 
    this.loadStudents();
  }

  resetSearch() {
    this.searchInput = '';
    this.keyword = '';    
    this.sortOption = '';
    this.currentPage = 1;
    this.loadStudents();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadStudents();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1; 
    this.loadStudents();  
  }

  private cdr = inject(ChangeDetectorRef);

  loadStudents() {
    this.studentService.getStudents(this.currentPage, this.pageSize, this.keyword, this.sortOption)
      .subscribe((response: any) => {
        this.students = response.data;
        this.totalPages = response.totalPages;
        this.cdr.detectChanges();
      });
  }

  downloadTemplate() {
    this.studentService.downloadTemplate().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Student_Import_Template.xlsx'; 
        document.body.appendChild(a);
        a.click();
      
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => alert('Lỗi tải file mẫu!')
    });
  }

  editStudent(student: any) {
    this.editingStudent = student; 
    this.isFormVisible = true;     
  }

  deleteStudent(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa học sinh này?')) {
      this.studentService.deleteStudent(id).subscribe({
        next: () => {
          alert('Xóa thành công!');
          if (this.students.length === 1 && this.currentPage > 1) {
            this.currentPage--;
          }
          this.loadStudents();
        },
        error: () => {
          alert('Xóa thất bại!');
        }
      });
    }
  }
}