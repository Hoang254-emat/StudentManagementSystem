# Student Management System UI

## Giới thiệu
Đây là giao diện người dùng (Client) của hệ thống quản lý học tập, được phát triển với **Angular**. Giao diện tập trung vào trải nghiệm người dùng (UX) hiện đại, hỗ trợ Dark Mode và tối ưu hóa các thao tác quản trị dữ liệu cho phía Admin.

## Công nghệ & Thư viện
* **Framework:** Angular 
* **Language:** TypeScript
* **Styling:** SCSS, Bootstrap Icons (cho các icon biểu tượng)
* **Components:** Standalone Components (Kiến trúc hiện đại không cần NgModule)
* **Notification:** ngx-toastr (thông báo chuyên nghiệp)
* **Data Handling:** Reactive Forms, RxJS (xử lý bất đồng bộ)

## Kiến trúc Frontend
Dự án được tổ chức theo mô hình Feature-based (theo nhóm chức năng):
1. **Components:** Mỗi module (Course, Class, Student...) được chia nhỏ thành các component riêng biệt để dễ bảo trì.
2. **Services:** Tách biệt logic gọi API và xử lý dữ liệu qua các service dùng chung (`HttpClient`).
3. **Interceptors:** Tự động đính kèm JWT Token vào Header của mọi yêu cầu HTTP gửi lên Backend.
4. **Guards:** Bảo vệ các Route, ngăn chặn người dùng chưa đăng nhập hoặc không có quyền truy cập.

## Tính năng nổi bật
* **Dark Mode Theme:** Giao diện tối màu "Tiên Hiệp" thân thiện với mắt, giảm mỏi khi làm việc lâu.
* **Smart Forms:** Validation dữ liệu thời gian thực (real-time) với `Reactive Forms`.
* **File Management:** Giao diện upload file trực quan, hỗ trợ kéo thả và đổi file giáo trình nhanh chóng.
* **Excel Import:** Hỗ trợ nhập liệu hàng loạt từ file Excel với template chuẩn hóa.
* **Responsive Layout:** Giao diện linh hoạt, hiển thị tốt trên nhiều kích thước màn hình.

## Các chức năng chính
- **Dashboard:** Tổng quan dữ liệu hệ thống.
- **Course Management:** Quản lý danh sách, xem/đổi/upload tài liệu giáo trình.
- **Data Tables:** Bảng hiển thị chuyên nghiệp với tính năng phân trang, tìm kiếm và sắp xếp.
- **Authentication:** Màn hình đăng nhập bảo mật, lưu trữ token an toàn.

## Yêu cầu hệ thống
* Node.js (phiên bản LTS mới nhất)
* Angular CLI 