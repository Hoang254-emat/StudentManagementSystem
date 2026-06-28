import { Component, inject, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassService } from '../class'; 
import { TeacherService } from '../../teacher/teacher';

@Component({
  selector: 'app-add-class',
  imports: [ReactiveFormsModule],
  templateUrl: './add-class.html',
  styleUrl: './add-class.scss',
  standalone: true,
})
export class AddClass implements OnChanges {
  private fb = inject(FormBuilder);
  private classService = inject(ClassService);
  private teacherService = inject(TeacherService);

  @Input() classData: any = null;
  @Output() classAdded = new EventEmitter<void>();

  teachers: any[] = [];

  classForm: FormGroup = this.fb.group({
    className: ['', Validators.required], 
    teacherId: ['', Validators.required] 
  });

  ngOnInit() {
    this.loadTeachers();
  }

  loadTeachers() {
    this.teacherService.getTeachers(1, 100).subscribe((res: any) => {
      this.teachers = res.data || [];
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['class'] && this.classData) {
      this.classForm.patchValue({
        className: this.classData.className,
        teacherId: this.classData.teacherId
      });
    } else {
      this.classForm.reset();
    }
  }

  onSubmit() {
    if (this.classForm.valid) {
      const formData = this.classForm.getRawValue(); 

      if (this.classData) {
        this.classService.updateClass(this.classData.id, formData).subscribe({
          next: () => {
            alert('Cập nhật lớp học thành công!');
            this.classAdded.emit();
          },
          error: () => alert('Cập nhật thất bại!')
        });
      } else {
        this.classService.addClass(formData).subscribe({
          next: () => {
            alert('Thêm lớp học thành công!');
            this.classAdded.emit();
          },
          error: () => alert('Thêm thất bại!')
        });
      }
    }
  }
}