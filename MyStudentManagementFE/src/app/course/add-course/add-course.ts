import { Component, inject, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CourseService } from '../course';
import { ClassService } from '../../class/class';
import { SubjectService } from '../../subject/subject';
import { TeacherService } from '../../teacher/teacher';

@Component({
  selector: 'app-add-course',
  imports: [ReactiveFormsModule],
  templateUrl: './add-course.html',
  styleUrl: './add-course.scss', 
  standalone: true
})
export class AddCourse implements OnInit, OnChanges {
  private fb = inject(FormBuilder);
  private courseService = inject(CourseService);
  private classService = inject(ClassService);
  private subjectService = inject(SubjectService);
  private teacherService = inject(TeacherService);

  @Input() courseData: any = null; 
  @Output() courseAdded = new EventEmitter<void>();

  classes: any[] = []; subjects: any[] = []; teachers: any[] = [];

  courseForm: FormGroup = this.fb.group({
    courseName: ['', [Validators.required, Validators.minLength(10)]],
    classId: ['', Validators.required],
    subjectId: ['', Validators.required],
    teacherId: ['', Validators.required]
  });

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    forkJoin({
      c: this.classService.getClasses(1, 100),
      s: this.subjectService.getSubjects(1, 100),
      t: this.teacherService.getTeachers(1, 100)
    }).subscribe(res => {
      this.classes = res.c.data || [];
      this.subjects = res.s.data || [];
      this.teachers = res.t.data || [];
    });
  }

// Trong AddCourse, hãy ép kiểu khi patchValue nếu cần
  ngOnChanges(changes: SimpleChanges) {
    if (changes['courseData'] && this.courseData) {
      this.courseForm.patchValue({
        courseName: this.courseData.courseName,
        classId: this.courseData.classId?.toString(),
        subjectId: this.courseData.subjectId?.toString(),
        teacherId: this.courseData.teacherId?.toString()
      });
    }
  }

  onSubmit() {
    if (this.courseForm.valid) {
      const formData = this.courseForm.getRawValue();

      if (this.courseData) {
        // Logic Cập nhật
        this.courseService.updateCourse(this.courseData.id, formData).subscribe({
          next: () => {
            alert('Cập nhật khóa học thành công!');
            this.courseAdded.emit();
          },
          error: () => alert('Cập nhật thất bại!')
        });
      } else {
        // Logic Thêm mới
        this.courseService.addCourse(formData).subscribe({
          next: () => {
            alert('Thêm khóa học thành công!');
            this.courseAdded.emit();
          },
          error: () => alert('Thêm thất bại!')
        });
      }
    }
  }
}