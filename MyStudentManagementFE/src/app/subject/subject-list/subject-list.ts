import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SubjectService } from '../subject';
import { AddSubject } from '../add-subject/add-subject';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../header/header';

@Component({
  selector: 'app-subject-list',
  imports: [AddSubject, FormsModule, HeaderComponent],
  templateUrl: './subject-list.html',
  styleUrl: './subject-list.scss',
  standalone: true,
})
export class SubjectList implements OnInit {
  private subjectService = inject(SubjectService);
  subjects: any[] = [];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  isFormVisible = false;
  editingSubject: any = null;

  searchInput: string = ''; 

  keyword: string = '';
  sortOption: string = '';

  ngOnInit() {
    this.loadSubjects();
  }

  toggleForm() {
    this.isFormVisible = !this.isFormVisible; 
    if (!this.isFormVisible) {
      this.editingSubject = null; 
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      if (confirm(`Bạn có chắc muốn import danh sách từ file ${file.name}?`)) {
        this.subjectService.importSubjectsFromExcel(file).subscribe({
          next: (res) => {
            alert(res.message);
            this.loadSubjects(); 
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

  onSubjectAdded() {
    this.isFormVisible = false;
    this.editingSubject = null;
    this.currentPage = 1; 
    this.loadSubjects(); 
  }

  onSearch() {
    this.keyword = this.searchInput; 
    this.currentPage = 1; 
    this.loadSubjects();
  }

  resetSearch() {
    this.searchInput = '';
    this.keyword = '';    
    this.sortOption = '';
    this.currentPage = 1;
    this.loadSubjects();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadSubjects();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1; 
    this.loadSubjects();  
  }

  private cdr = inject(ChangeDetectorRef);

  loadSubjects() {
    this.subjectService.getSubjects(this.currentPage, this.pageSize, this.keyword, this.sortOption)
      .subscribe((response: any) => {
        this.subjects = response.data;
        this.totalPages = response.totalPages;
        this.cdr.detectChanges();
      });
  }

  downloadTemplate() {
    this.subjectService.downloadTemplate().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Subject_Import_Template.xlsx'; 
        document.body.appendChild(a);
        a.click();
      
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => alert('Lỗi tải file mẫu!')
    });
  }

  editSubject(subject: any) {
    this.editingSubject = subject; 
    this.isFormVisible = true;     
  }

  deleteSubject(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa môn học này?')) {
      this.subjectService.deleteSubject(id).subscribe({
        next: () => {
          alert('Xóa thành công!');
          if (this.subjects.length === 1 && this.currentPage > 1) {
            this.currentPage--;
          }
          this.loadSubjects();
        },
        error: () => {
          alert('Xóa thất bại!');
        }
      });
    }
  }
}