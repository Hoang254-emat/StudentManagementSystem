import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { TeacherService } from '../teacher';
import { AddTeacher } from '../add-teacher/add-teacher';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../header/header';

@Component({
  selector: 'app-teacher-list',
  imports: [AddTeacher, FormsModule, HeaderComponent],
  templateUrl: './teacher-list.html',
  styleUrl: './teacher-list.scss',
  standalone: true,
})
export class TeacherList implements OnInit {
  private teacherService = inject(TeacherService);
  teachers: any[] = [];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  isFormVisible = false;
  editingTeacher: any = null;

  searchInput: string = ''; 

  keyword: string = '';
  sortOption: string = '';

  ngOnInit() {
    this.loadTeachers();
  }

  toggleForm() {
    this.isFormVisible = !this.isFormVisible; 
    if (!this.isFormVisible) {
      this.editingTeacher = null; 
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      if (confirm(`Bạn có chắc muốn import danh sách từ file ${file.name}?`)) {
        this.teacherService.importTeachersFromExcel(file).subscribe({
          next: (res) => {
            alert(res.message);
            this.loadTeachers(); 
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

  onTeacherAdded() {
    this.isFormVisible = false;
    this.editingTeacher = null;
    this.currentPage = 1; 
    this.loadTeachers(); 
  }

  onSearch() {
    this.keyword = this.searchInput; 
    this.currentPage = 1; 
    this.loadTeachers();
  }

  resetSearch() {
    this.searchInput = '';
    this.keyword = '';    
    this.sortOption = '';
    this.currentPage = 1;
    this.loadTeachers();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadTeachers();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1; 
    this.loadTeachers();  
  }

  private cdr = inject(ChangeDetectorRef);

  loadTeachers() {
    this.teacherService.getTeachers(this.currentPage, this.pageSize, this.keyword, this.sortOption)
      .subscribe((response: any) => {
        this.teachers = response.data;
        this.totalPages = response.totalPages;
        this.cdr.detectChanges();
      });
  }

  downloadTemplate() {
    this.teacherService.downloadTemplate().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Teacher_Import_Template.xlsx'; 
        document.body.appendChild(a);
        a.click();
      
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => alert('Lỗi tải file mẫu!')
    });
  }

  editTeacher(teacher: any) {
    this.editingTeacher = teacher; 
    this.isFormVisible = true;     
  }

  deleteTeacher(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa giáo viên này?')) {
      this.teacherService.deleteTeacher(id).subscribe({
        next: () => {
          alert('Xóa thành công!');
          if (this.teachers.length === 1 && this.currentPage > 1) {
            this.currentPage--;
          }
          this.loadTeachers();
        },
        error: () => {
          alert('Xóa thất bại!');
        }
      });
    }
  }
}