import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './auth.html',
  styleUrl: './auth.scss'
})
export class AuthComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoginMode = true; 
  registerRole: 'student' | 'teacher' = 'student'; 

  authForm: FormGroup = this.fb.group({
    id: [''], 
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.authForm.reset();
    
    if (!this.isLoginMode) {
      this.authForm.get('id')?.setValidators([Validators.required]);
    } else {
      this.authForm.get('id')?.clearValidators();
    }
    this.authForm.get('id')?.updateValueAndValidity();
  }

  setRegisterRole(role: 'student' | 'teacher') {
    this.registerRole = role;
  }

  onSubmit() {
    if (this.authForm.invalid) return;

    const formValue = this.authForm.value;

    if (this.isLoginMode) {
      // XỬ LÝ ĐĂNG NHẬP
      const loginData = { username: formValue.username, password: formValue.password };
      this.authService.login(loginData).subscribe({
        next: (res) => {
          alert('Đăng nhập thành công!');
          this.router.navigate(['/dashboard']);
        },
        error: (err) => alert('Tài khoản hoặc mật khẩu không chính xác!')
      });
    } else {
      // XỬ LÝ ĐĂNG KÝ (KÍCH HOẠT TÀI KHOẢN THEO ID)
      const registerData = { 
        id: formValue.id, 
        username: formValue.username, 
        password: formValue.password 
      };

      // Tự động phân nhánh API dựa vào tab vai trò đang được chọn
      const request$ = this.registerRole === 'student'
        ? this.authService.registerStudent(registerData)
        : this.authService.registerTeacher(registerData);

      request$.subscribe({
        next: () => {
          const roleName = this.registerRole === 'student' ? 'Sinh viên' : 'Giáo viên';
          alert(`Kích hoạt tài khoản ${roleName} thành công! Vui lòng đăng nhập.`);
          this.toggleMode(); 
        },
        error: (err) => {
          alert('Kích hoạt tài khoản thất bại: ' + (err.error?.message || err.error || 'Mã ID không hợp lệ'));
        }
      });
    }
  }
}