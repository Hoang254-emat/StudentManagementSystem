# Student Management System

Hệ thống quản lý khóa học và giáo trình dành cho sinh viên.

## Cấu trúc dự án
- `/MyWebApi`: Chứa mã nguồn Web API (.NET).
- `/MyStudentMangenentFE`: Chứa mã nguồn giao diện (Angular).

## Các công nghệ sử dụng
- **Backend**: .NET 10 Web API, Entity Framework Core, SQL Server.
- **Frontend**: Angular, SCSS, ngx-toastr, Bootstrap Icons.

## Student Management System UI (Frontend)

### Giới thiệu
Đây là giao diện người dùng (Client) của hệ thống quản lý học tập, được phát triển với **Angular**. Giao diện tập trung vào trải nghiệm người dùng (UX) hiện đại, hỗ trợ Dark Mode và tối ưu hóa các thao tác quản trị dữ liệu cho phía Admin.

### Công nghệ & Thư viện
* **Framework:** Angular 
* **Language:** TypeScript
* **Styling:** SCSS, Bootstrap Icons 
* **Components:** Standalone Components 
* **Data Handling:** Reactive Forms, RxJS 

### Kiến trúc Frontend
Dự án được tổ chức theo mô hình Feature-based (theo nhóm chức năng):
1. **Components:** Mỗi module (Course, Class, Student...) được chia nhỏ thành các component riêng biệt để dễ bảo trì.
2. **Services:** Tách biệt logic gọi API và xử lý dữ liệu qua các service dùng chung (`HttpClient`).
3. **Interceptors:** Tự động đính kèm JWT Token vào Header của mọi yêu cầu HTTP gửi lên Backend.
4. **Guards:** Bảo vệ các Route, ngăn chặn người dùng chưa đăng nhập hoặc không có quyền truy cập.

### Tính năng nổi bật
* **Dark Mode Theme:** Giao diện tối màu "Tiên Hiệp" thân thiện với mắt, giảm mỏi khi làm việc lâu.
* **Smart Forms:** Validation dữ liệu thời gian thực (real-time) với `Reactive Forms`.
* **File Management:** Giao diện upload file trực quan, hỗ trợ kéo thả và đổi file giáo trình nhanh chóng.
* **Excel Import:** Hỗ trợ nhập liệu hàng loạt từ file Excel với template chuẩn hóa.
* **Responsive Layout:** Giao diện linh hoạt, hiển thị tốt trên nhiều kích thước màn hình.

### Các chức năng chính
- **Dashboard:** Tổng quan dữ liệu hệ thống.
- **Course Management:** Quản lý danh sách, xem/đổi/upload tài liệu giáo trình.
- **Data Tables:** Bảng hiển thị chuyên nghiệp với tính năng phân trang, tìm kiếm và sắp xếp.
- **Authentication:** Màn hình đăng nhập bảo mật, lưu trữ token an toàn.

### Yêu cầu hệ thống
* Node.js (phiên bản LTS mới nhất)
* Angular CLI 

## Student Management System API (Backend)

### Giới thiệu
Đây là hệ thống quản lý học tập được xây dựng trên nền tảng **.NET 10**. Hệ thống tập trung vào việc quản lý thông tin sinh viên, giảng viên, khóa học và danh mục môn học, hỗ trợ các thao tác quản trị dữ liệu hiệu quả và bảo mật.

### Công nghệ & Thư viện
* **Framework:** .NET 10 Web API
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Token)
* **Caching:** MemoryCache
* **Mapping:** AutoMapper
* **API Documentation:** Swagger/OpenAPI

### Kiến trúc hệ thống
Dự án áp dụng mô hình **3-Layer Architecture** để tách biệt trách nhiệm:
1. **Controller Layer:** Tiếp nhận yêu cầu HTTP, kiểm soát quyền truy cập và trả về kết quả.
2. **Service Layer:** Chứa logic nghiệp vụ, chuyển đổi DTO và tương tác với các service khác (như FileService).
3. **Repository Layer:** Tương tác trực tiếp với Database thông qua Entity Framework Core.

### Tính năng nổi bật
* **Phân trang (Pagination):** Tất cả các API danh sách đều hỗ trợ `page` và `pageSize`.
* **Bộ lọc & Sắp xếp:** Hỗ trợ tìm kiếm theo từ khóa (tên/ID) và sắp xếp dữ liệu linh hoạt.
* **Caching:** Sử dụng `IMemoryCache` để giảm tải truy vấn DB đối với dữ liệu ít thay đổi.
* **Bảo mật:** Phân quyền chặt chẽ giữa `Admin` và `User` thông qua JWT roles.
* **Tài liệu API:** Tích hợp Swagger để dễ dàng kiểm thử và theo dõi API.

### Danh sách API chính

| Nhóm chức năng | Endpoint chính | Ghi chú |
| :--- | :--- | :--- |
| **Auth** | `/api/Auth` | Đăng ký, Đăng nhập (JWT) |
| **Class** | `/api/Class` | Phân trang, CRUD cho Admin |
| **Course** | `/api/Course` | Phân trang, CRUD, Upload tài liệu |
| **Student** | `/api/Student` | Phân trang, CRUD, Avatar, Profile |
| **Subject** | `/api/Subject` | Danh mục môn học, quản trị bởi Admin |
| **Teacher** | `/api/Teacher` | Phân trang, CRUD, Avatar, Profile |

### Yêu cầu hệ thống
* .NET 10 SDK
* SQL Server

## Hướng dẫn chạy dự án
1. Clone repo về máy.
2. Truy cập vào folder `/MyWebApi`, cấu hình `appsettings.json` và chạy `dotnet run`.
3. Truy cập vào folder `/MyStudentMangenentFE`, chạy `npm install` và `ng serve`.