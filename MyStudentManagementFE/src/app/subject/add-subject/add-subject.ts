import { Component, inject, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SubjectService } from '../subject'; 
import { TeacherService } from '../../teacher/teacher';

@Component({
  selector: 'app-add-subject',
  imports: [ReactiveFormsModule],
  templateUrl: './add-subject.html',
  styleUrl: './add-subject.scss',
  standalone: true,
})
export class AddSubject implements OnChanges {
  private fb = inject(FormBuilder);
  private subjectService = inject(SubjectService);
  private teacherService = inject(TeacherService);

  @Input() subject: any = null;
  @Output() subjectAdded = new EventEmitter<void>();

  teachers: any[] = [];

  subjectForm: FormGroup = this.fb.group({
    id: ['', Validators.required],
    subjectName: ['', Validators.required], 
    credits: [3, [Validators.required, Validators.min(1)]],
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
    if (changes['subject'] && this.subject) {
      this.subjectForm.patchValue({
        id: this.subject.id,
        subjectName: this.subject.subjectName,
        credits: this.subject.credits,
        teacherId: this.subject.teacherId
      });
      this.subjectForm.get('id')?.disable();
    } else {
      this.subjectForm.reset();
      this.subjectForm.get('id')?.enable(); 
    }
  }

  onSubmit() {
    if (this.subjectForm.valid) {
      const formData = this.subjectForm.getRawValue(); 

      if (this.subject) {
        this.subjectService.updateSubject(formData.id, formData).subscribe({
          next: () => {
            alert('Cập nhật môn học thành công!');
            this.subjectAdded.emit();
          },
          error: () => alert('Cập nhật thất bại!')
        });
      } else {
        this.subjectService.addSubject(formData).subscribe({
          next: () => {
            alert('Thêm môn học thành công!');
            this.subjectAdded.emit();
          },
          error: () => alert('Thêm thất bại!')
        });
      }
    }
  }
}