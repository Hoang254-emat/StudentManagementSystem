import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ClassService } from '../class';
import { AddClass } from '../add-class/add-class';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../header/header';

@Component({
  selector: 'app-class-list',
  imports: [AddClass, FormsModule, HeaderComponent],
  templateUrl: './class-list.html',
  styleUrl: './class-list.scss',
  standalone: true,
})
export class ClassList implements OnInit {
  private classService = inject(ClassService);
  classes: any[] = [];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  isFormVisible = false;
  editingClass: any = null;

  searchInput: string = ''; 

  keyword: string = '';
  sortOption: string = '';

  ngOnInit() {
    this.loadClasses();
  }

  toggleForm() {
    this.isFormVisible = !this.isFormVisible; 
    if (!this.isFormVisible) {
      this.editingClass = null; 
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      if (confirm(`Bạn có chắc muốn import danh sách từ file ${file.name}?`)) {
        this.classService.importClassesFromExcel(file).subscribe({
          next: (res) => {
            alert(res.message);
            this.loadClasses(); 
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

  onClassAdded() {
    this.isFormVisible = false;
    this.editingClass = null;
    this.currentPage = 1; 
    this.loadClasses(); 
  }

  onSearch() {
    this.keyword = this.searchInput; 
    this.currentPage = 1; 
    this.loadClasses();
  }

  resetSearch() {
    this.searchInput = '';
    this.keyword = '';    
    this.sortOption = '';
    this.currentPage = 1;
    this.loadClasses();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadClasses();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1; 
    this.loadClasses();  
  }

  private cdr = inject(ChangeDetectorRef);

  loadClasses() {
    this.classService.getClasses(this.currentPage, this.pageSize, this.keyword, this.sortOption)
      .subscribe((response: any) => {
        this.classes = response.data;
        this.totalPages = response.totalPages;
        this.cdr.detectChanges();
      });
  }

  downloadTemplate() {
    this.classService.downloadTemplate().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Class_Import_Template.xlsx'; 
        document.body.appendChild(a);
        a.click();
      
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => alert('Lỗi tải file mẫu!')
    });
  }

  editClass(classData: any) {
    this.editingClass = classData; 
    this.isFormVisible = true;     
  }

  deleteClass(id: number) {
    if (confirm('Bạn có chắc chắn muốn xóa lớp học này?')) {
      this.classService.deleteClass(id).subscribe({
        next: () => {
          alert('Xóa thành công!');
          if (this.classes.length === 1 && this.currentPage > 1) {
            this.currentPage--;
          }
          this.loadClasses();
        },
        error: () => {
          alert('Xóa thất bại!');
        }
      });
    }
  }
}