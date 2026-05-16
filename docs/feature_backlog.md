# Feature Backlog

Danh sách này giúp chia nhỏ việc code với AI. Khi làm chức năng nào, chỉ lấy đúng phần đó để tránh lan man.

## Nền tảng kỹ thuật

- Chuyển connection string ra khỏi `ApplicationDbContext.OnConfiguring`.
- Đăng ký `ApplicationDbContext` trong `Program.cs`.
- Tạo cấu trúc `ViewModels/`.
- Tạo helper/service sinh mã khóa chính dạng `ND001`, `BN001`, `BS001`, ...
- Thêm layout phân quyền theo vai trò.
- Thêm seed data mẫu hoặc script insert data mẫu.

## Module tài khoản

- Đăng ký bệnh nhân.
- Đăng nhập/đăng xuất.
- Hash mật khẩu.
- Trang thông tin cá nhân.
- Cập nhật thông tin cá nhân.
- Chặn tài khoản bị khóa.
- Phân quyền route theo vai trò.

## Module đặt lịch

- Danh sách chuyên khoa.
- Danh sách bác sĩ theo chuyên khoa.
- Xem lịch làm việc bác sĩ.
- Đặt lịch khám.
- Kiểm tra ca khám hợp lệ.
- Danh sách lịch hẹn của bệnh nhân.
- Đổi lịch hẹn.
- Hủy lịch hẹn.
- Bác sĩ xem lịch khám trong ngày.

## Module khám bệnh

- Bác sĩ tiếp nhận bệnh nhân.
- Lập phiếu khám.
- Chỉ định dịch vụ khám.
- Kê đơn thuốc.
- Cập nhật trạng thái phiếu khám.
- Bệnh nhân xem kết quả khám.
- Bệnh nhân xem đơn thuốc.

## Module thanh toán

- Tính chi phí khám.
- Tính chi phí thuốc.
- Tạo hóa đơn.
- Xem chi tiết hóa đơn.
- Cập nhật trạng thái thanh toán.
- Lịch sử thanh toán của bệnh nhân.
- Báo cáo doanh thu theo ngày/tháng.

## Module quản trị

- CRUD chuyên khoa.
- CRUD phòng khám.
- CRUD bác sĩ.
- CRUD lịch làm việc.
- CRUD dịch vụ khám.
- CRUD thuốc.
- Quản lý bệnh nhân.
- Quản lý người dùng và khóa/mở tài khoản.
- Báo cáo tổng hợp.

## Thứ tự triển khai khuyến nghị

1. Cấu hình database và bảo mật connection string.
2. Authentication cơ bản.
3. Quản lý chuyên khoa, phòng khám, bác sĩ, lịch làm việc.
4. Đặt lịch khám.
5. Bác sĩ lập phiếu khám và kê đơn.
6. Hóa đơn/thanh toán.
7. Báo cáo và hoàn thiện giao diện.
