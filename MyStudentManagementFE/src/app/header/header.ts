import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth/auth.service'; 
import { StudentService } from '../student/student';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  private authService = inject(AuthService);
  private studentService = inject(StudentService);
  private router = inject(Router);

  isDropdownOpen = false; 
  userInfo: any = null;

  ngOnInit() {
    this.studentService.getMyProfile().subscribe({
      next: (data) => {
        this.userInfo = data;
      },
      error: () => {
        this.userInfo = null;
      }
    });
  }  

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  viewProfile() {
    this.isDropdownOpen = false;
    
    const token = localStorage.getItem('token'); 
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        const role = decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

        if (role === 'Teacher') {
          this.router.navigate(['/teacher/profile']);
        } else {
          this.router.navigate(['/student/profile']);
        }
      } catch (error) {
        console.error("Token invalid", error);
        this.router.navigate(['/login']);
      }
    }
  }

  logout() {
    this.authService.logout(); 
    this.router.navigate(['/login']); 
  }
}