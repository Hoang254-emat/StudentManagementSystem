import { Component, inject, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TeacherService } from '../teacher';

@Component({
  selector: 'app-add-teacher',
  imports: [ReactiveFormsModule],
  templateUrl: './add-teacher.html',
  styleUrl: './add-teacher.scss',
  standalone: true,
})
export class AddTeacher implements OnChanges {
  private fb = inject(FormBuilder);
  private teacherService = inject(TeacherService);

  @Input() teacher: any = null;
  @Output() teacherAdded = new EventEmitter<void>();

  teacherForm: FormGroup = this.fb.group({
    id: ['', Validators.required],
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  ngOnChanges(changes: SimpleChanges) {
    if (changes['teacher'] && this.teacher) {
      this.teacherForm.patchValue({
        id: this.teacher.id,
        name: this.teacher.name,
        email: this.teacher.email
      });
      this.teacherForm.get('id')?.disable();
    } else {
      this.teacherForm.reset();
      this.teacherForm.get('id')?.enable(); 
    }
  }

onSubmit() {
    if (this.teacherForm.valid) {
      const formData = this.teacherForm.getRawValue(); 

      if (this.teacher) {
        this.teacherService.updateTeacher(formData.id, formData).subscribe({
          next: () => {
            alert('Cập nhật thành công!');
            this.teacherForm.reset();
            this.teacherAdded.emit();
          },
          error: () => alert('Cập nhật thất bại!')
        });
      } else {
        this.teacherService.addTeacher(formData).subscribe({
          next: () => {
            alert('Thêm thành công!');
            this.teacherForm.reset();
            this.teacherAdded.emit();
          },
          error: () => alert('Thêm thất bại!')
        });
      }
    }
  }
}
