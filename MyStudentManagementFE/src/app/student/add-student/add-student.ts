import { Component, inject, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StudentService } from '../student';


@Component({
  selector: 'app-add-student',
  imports: [ReactiveFormsModule],
  templateUrl: './add-student.html',
  styleUrl: './add-student.scss',
  standalone: true,
})
export class AddStudent implements OnChanges {
  private fb = inject(FormBuilder);
  private studentService = inject(StudentService);

  @Input() student: any = null;
  @Output() studentAdded = new EventEmitter<void>();

  studentForm: FormGroup = this.fb.group({
    id: ['', Validators.required],
    name: ['', Validators.required],
    birthday: ['', Validators.required],
    classId: [null, [Validators.required, Validators.min(1)]]
  });

  ngOnChanges(changes: SimpleChanges) {
    if (changes['student'] && this.student) {
      this.studentForm.patchValue({
        id: this.student.id,
        name: this.student.name,
        birthday: this.student.birthday ? this.student.birthday.split('T')[0] : '', 
        classId: this.student.classId
      });
      this.studentForm.get('id')?.disable();
    } else {
      this.studentForm.reset();
      this.studentForm.get('id')?.enable(); 
    }
  }

onSubmit() {
    if (this.studentForm.valid) {
      // Dùng getRawValue() để lấy cả giá trị của ô ID đang bị disable
      const formData = this.studentForm.getRawValue(); 

      if (this.student) {
        // GỌI API SỬA (PUT)
        this.studentService.updateStudent(formData.id, formData).subscribe({
          next: () => {
            alert('Cập nhật thành công!');
            this.studentForm.reset();
            this.studentAdded.emit();
          },
          error: () => alert('Cập nhật thất bại!')
        });
      } else {
        // GỌI API THÊM MỚI (POST)
        this.studentService.addStudent(formData).subscribe({
          next: () => {
            alert('Thêm thành công!');
            this.studentForm.reset();
            this.studentAdded.emit();
          },
          error: () => alert('Thêm thất bại!')
        });
      }
    }
  }
}
