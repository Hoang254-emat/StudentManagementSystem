import { Component, OnInit, inject, ChangeDetectorRef, Input } from '@angular/core';
import { StudentService } from '../student/student';
import { TeacherService } from '../teacher/teacher';
import { DatePipe } from '@angular/common';
import { HeaderComponent } from '../header/header';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [DatePipe, HeaderComponent], 
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private studentService = inject(StudentService);
  private teacherService = inject(TeacherService);
  private cdr = inject(ChangeDetectorRef);
  private route = inject(ActivatedRoute);
  
  @Input() userType: 'student' | 'teacher' = 'student'; 
  profileInfo: any = null;

  ngOnInit() {
    this.userType = this.route.snapshot.data['userType'] || 'student';
    this.loadProfile();
  }

  loadProfile() {
    const service = this.userType === 'student' ? this.studentService : this.teacherService;
    service.getMyProfile().subscribe({
      next: (data) => {
        this.profileInfo = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Lỗi lấy hồ sơ:', err)
    });
  }

  viewAvatar() {
    const url = this.profileInfo.avatarUrl 
      ? 'https://localhost:7294' + this.profileInfo.avatarUrl 
      : `https://ui-avatars.com/api/?name=${this.profileInfo.name}&background=22252e&color=d4af37&size=500`;
    window.open(url, '_blank');
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const service = this.userType === 'student' ? this.studentService : this.teacherService;
      service.uploadAvatar(this.profileInfo.id, file).subscribe({
        next: (res: any) => {
          this.profileInfo.avatarUrl = res.url || res.avatarUrl || res;
          this.cdr.detectChanges();
          alert('Cập nhật diện mạo thành công!');
        },
        error: () => alert('Lỗi khi tải ảnh lên!')
      });
    }
  }
}