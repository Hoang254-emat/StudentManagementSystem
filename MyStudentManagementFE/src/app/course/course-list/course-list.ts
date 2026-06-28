import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CourseService } from '../course'; 
import { AddCourse } from '../add-course/add-course'; 
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../header/header';
import { ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-course-list',
  imports: [AddCourse, FormsModule, HeaderComponent],
  templateUrl: './course-list.html',
  styleUrl: './course-list.scss', 
  standalone: true,
})
export class CourseList implements OnInit {
  private courseService = inject(CourseService);
  courses: any[] = [];

  currentPage = 1;
  pageSize = 5;
  totalPages = 0;

  isFormVisible = false;
  editingCourse: any = null;

  searchInput: string = ''; 
  keyword: string = '';
  sortOption: string = '';

  ngOnInit() {
    this.loadCourses();
  }

  toggleForm() {
    this.isFormVisible = !this.isFormVisible; 
    if (!this.isFormVisible) {
      this.editingCourse = null; 
    }
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      if (confirm(`Bạn có chắc muốn import danh sách từ file ${file.name}?`)) {
        this.courseService.importCoursesFromExcel(file).subscribe({
          next: (res) => {
            alert(res.message);
            this.loadCourses(); 
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

  onCourseAdded() {
    this.isFormVisible = false;
    this.editingCourse = null;
    this.currentPage = 1; 
    this.loadCourses(); 
  }

  onSearch() {
    this.keyword = this.searchInput; 
    this.currentPage = 1; 
    this.loadCourses();
  }

  resetSearch() {
    this.searchInput = '';
    this.keyword = '';    
    this.sortOption = '';
    this.currentPage = 1;
    this.loadCourses();
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadCourses();
    }
  }

  onPageSizeChange() {
    this.currentPage = 1; 
    this.loadCourses();  
  }

  private cdr = inject(ChangeDetectorRef);

  loadCourses() {
    this.courseService.getCourses(this.currentPage, this.pageSize, this.keyword, this.sortOption)
      .subscribe((response: any) => {
        this.courses = response.data;
        this.totalPages = response.totalPages;
        this.cdr.detectChanges();
      });
  }

  downloadTemplate() {
    this.courseService.downloadTemplate().subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Course_Import_Template.xlsx'; 
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error: () => alert('Lỗi tải file mẫu!')
    });
  }

  editCourse(courseData: any) {
    this.editingCourse = courseData; 
    this.isFormVisible = true;     
  }

  deleteCourse(id: number) {
    if (confirm('Bạn có chắc chắn muốn xóa khóa học này?')) {
      this.courseService.deleteCourse(id).subscribe({
        next: () => {
          alert('Xóa thành công!');
          if (this.courses.length === 1 && this.currentPage > 1) {
            this.currentPage--;
          }
          this.loadCourses();
        },
        error: () => {
          alert('Xóa thất bại!');
        }
      });
    }
  }

  @ViewChild('fileInput') fileInput!: ElementRef;
  selectedCourseId: number | null = null;

  triggerUpload(id: number){
    this.selectedCourseId = id;
    this.fileInput.nativeElement.click();
  }

  onFileUploaded(event: any) {
    const file: File = event.target.files[0];
    if (file && this.selectedCourseId){
      this.courseService.uploadCurriculum(this.selectedCourseId, file).subscribe({
        next: (res) => {
          alert('Upload thành công!');
          this.loadCourses();
          this.selectedCourseId = null;
        },
        error: () => {
          alert('Upload thất bại!');
          this.selectedCourseId = null;
        }
      });
    }
    event.target.value = '';
  }
}