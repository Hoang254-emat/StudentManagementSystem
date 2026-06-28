import { Routes } from '@angular/router';
import { AuthComponent } from './auth/auth/auth';
import { StudentList } from './student/student-list/student-list';
import { Dashboard } from './dashboard/dashboard';
import { ProfileComponent } from './profile/profile';
import { TeacherList } from './teacher/teacher-list/teacher-list';
import { SubjectList } from './subject/subject-list/subject-list';
import { ClassList } from './class/class-list/class-list';
import { CourseList } from './course/course-list/course-list';

export const routes: Routes = [
    { path: 'login', component: AuthComponent },
    { path: 'dashboard', component: Dashboard },
    { path: 'students', component: StudentList },
    { path: 'teachers', component: TeacherList },
    { path: 'subjects', component: SubjectList},
    { path: 'classes', component: ClassList},
    { path: 'courses', component: CourseList},
    { path: 'student/profile', component: ProfileComponent, data: { userType: 'student' } },
    { path: 'teacher/profile', component: ProfileComponent, data: { userType: 'teacher' } },    
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login'}
];
