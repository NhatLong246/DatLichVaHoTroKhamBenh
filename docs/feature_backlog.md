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

## Cập nhật tiến độ module đặt lịch

Đã hoàn thành:

- Danh sách chuyên khoa trong trang đặt lịch.
- Danh sách bác sĩ theo chuyên khoa.
- Xem lịch làm việc bác sĩ.
- Gợi ý chuyên khoa theo triệu chứng bằng rule-based.
- Chọn ngày, ca, khung giờ và thời lượng khám.
- Đặt lịch khám và tạo `DangKyLichKham`.
- Kiểm tra ca/giờ khám hợp lệ, phòng hoạt động, ngày không quá khứ, không trùng giờ cùng bác sĩ.
- Hiển thị danh sách lịch hẹn của bệnh nhân, gồm ngày/ca/giờ/thời lượng, bác sĩ, chuyên khoa, phòng khám và trạng thái.
- Hủy lịch hẹn bằng modal custom khi lịch còn `Chờ khám`, chưa tới giờ khám và chưa có phiếu khám.

Việc cần làm tiếp:

- Cho phép đổi lịch hẹn nếu nghiệp vụ yêu cầu.
- Bác sĩ xem lịch khám theo ngày/ca/giờ.
- Quản trị viên quản lý lịch làm việc bác sĩ và sức chứa phòng.
- Có thể nâng cấp gợi ý chuyên khoa sang AI API sau này, nhưng vẫn phải đối chiếu với bảng `ChuyenKhoa`.

## Cập nhật tiến độ màn hình bệnh nhân

Đã hoàn thành:

- Dashboard bệnh nhân nối tới các màn hình chính: đặt lịch, lịch hẹn, hồ sơ bệnh án, hóa đơn, cài đặt.
- Trang `LichKham/QuanLy`: theo dõi lịch sử đặt khám và hủy lịch hợp lệ.
- Trang `HoSoBenhAn/Index`: xem lịch sử khám bệnh, chi tiết khám, đơn thuốc và dịch vụ đã dùng.
- Trang `HoaDon/Index`: xem danh sách/chi tiết hóa đơn, xác nhận thanh toán hóa đơn `Chưa thanh toán`.
- Trang `CaiDat/Index`: xem thông tin cá nhân/tài khoản, cập nhật điện thoại/địa chỉ, đổi mật khẩu.
- `wwwroot/js/dashboard.js` đã mở rộng modal custom cho các form xác nhận như đăng xuất, hủy lịch, thanh toán và đổi mật khẩu.

Việc cần làm tiếp:

- Tạo dữ liệu thật hoặc seed data đầy đủ cho `PhieuKham`, `DonThuoc`, `ChiTietDonThuoc`, `ChiTietDichVuKham`, `HoaDon`, `ChiTietHoaDon` để demo đủ các tab.
- Hoàn thiện luồng bác sĩ lập phiếu khám, kê đơn, chỉ định dịch vụ.
- Hoàn thiện luồng tạo hóa đơn tự động từ chi phí dịch vụ và thuốc.
- Cân nhắc chính sách cho sửa thông tin cá nhân nhạy cảm như họ tên, ngày sinh, giới tính nếu sau này cần quy trình xác minh.
