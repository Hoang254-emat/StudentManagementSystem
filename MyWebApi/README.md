# Student Management System API

## Giới thiệu
Đây là hệ thống quản lý học tập được xây dựng trên nền tảng **.NET 10**. Hệ thống tập trung vào việc quản lý thông tin sinh viên, giảng viên, khóa học và danh mục môn học, hỗ trợ các thao tác quản trị dữ liệu hiệu quả và bảo mật.

## Công nghệ & Thư viện
* **Framework:** .NET 10 Web API
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Token)
* **Caching:** MemoryCache
* **Mapping:** AutoMapper
* **API Documentation:** Swagger/OpenAPI

## Kiến trúc hệ thống
Dự án áp dụng mô hình **3-Layer Architecture** để tách biệt trách nhiệm:
1. **Controller Layer:** Tiếp nhận yêu cầu HTTP, kiểm soát quyền truy cập và trả về kết quả.
2. **Service Layer:** Chứa logic nghiệp vụ, chuyển đổi DTO và tương tác với các service khác (như FileService).
3. **Repository Layer:** Tương tác trực tiếp với Database thông qua Entity Framework Core.

## Tính năng nổi bật
* **Phân trang (Pagination):** Tất cả các API danh sách đều hỗ trợ `page` và `pageSize`.
* **Bộ lọc & Sắp xếp:** Hỗ trợ tìm kiếm theo từ khóa (tên/ID) và sắp xếp dữ liệu linh hoạt.
* **Caching:** Sử dụng `IMemoryCache` để giảm tải truy vấn DB đối với dữ liệu ít thay đổi.
* **Bảo mật:** Phân quyền chặt chẽ giữa `Admin` và `User` thông qua JWT roles.
* **Tài liệu API:** Tích hợp Swagger để dễ dàng kiểm thử và theo dõi API.

## Danh sách API chính

| Nhóm chức năng | Endpoint chính | Ghi chú |
| :--- | :--- | :--- |
| **Auth** | `/api/Auth` | Đăng ký, Đăng nhập (JWT) |
| **Class** | `/api/Class` | Phân trang, CRUD cho Admin |
| **Course** | `/api/Course` | Phân trang, CRUD, Upload tài liệu |
| **Student** | `/api/Student` | Phân trang, CRUD, Avatar, Profile |
| **Subject** | `/api/Subject` | Danh mục môn học, quản trị bởi Admin |
| **Teacher** | `/api/Teacher` | Phân trang, CRUD, Avatar, Profile |

## Yêu cầu hệ thống
* .NET 10 SDK
* SQL Server
