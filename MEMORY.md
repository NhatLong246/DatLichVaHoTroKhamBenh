# MEMORY - HeThongDatLichVaKhamBenh

File này là bộ nhớ ngắn hạn/dài hạn cho AI khi hỗ trợ code dự án. Trước khi sửa code, hãy đọc file này cùng `system_rules.md` và các tài liệu trong `docs/`.

## Tổng quan dự án

- Tên dự án: `HeThongDatLichVaKhamBenh`
- Loại ứng dụng: ASP.NET Core MVC quản lý đặt lịch và hỗ trợ khám bệnh.
- Target framework hiện tại: `.NET 9.0`.
- ORM: Entity Framework Core với SQL Server.
- Database script chính: `btl_table.sql`.
- DbContext: `Models/EF/ApplicationDbContext.cs`.
- Entity models: `Models/Entities/`.
- Giao diện: Razor Views trong `Views/`, static assets trong `wwwroot/`.

## Mục tiêu hệ thống

Hệ thống hỗ trợ bệnh nhân đặt lịch khám, bác sĩ tiếp nhận và lập phiếu khám/kê đơn, quản trị viên quản lý dữ liệu nền và xem báo cáo. Các phân hệ chính:

1. Quản lý người dùng
2. Quản lý lịch khám
3. Quản lý khám bệnh
4. Quản lý thanh toán
5. Quản trị hệ thống

## Vai trò người dùng

- `Bệnh nhân`: đăng ký/đăng nhập, cập nhật thông tin cá nhân, đặt/đổi/hủy lịch khám, xem kết quả khám, xem đơn thuốc, xem hóa đơn và lịch sử thanh toán.
- `Bác sĩ`: xem lịch làm việc, xem danh sách bệnh nhân theo lịch, tiếp nhận bệnh nhân, lập phiếu khám, kê đơn thuốc, chỉ định dịch vụ khám, cập nhật hồ sơ bệnh án.
- `Quản trị`: quản lý người dùng, bác sĩ, chuyên khoa, phòng khám, dịch vụ, lịch làm việc, thuốc, dữ liệu bệnh nhân và báo cáo thống kê.

## Database chính

Database: `HeThongDatLichVaKhamBenh`

Các bảng hiện có:

- `NguoiDung`: tài khoản đăng nhập, vai trò, trạng thái.
- `ChuyenKhoa`: danh mục chuyên khoa.
- `PhongKham`: phòng khám thuộc chuyên khoa.
- `DichVuKham`: dịch vụ khám thuộc chuyên khoa.
- `BacSi`: thông tin bác sĩ, liên kết 1-1 với `NguoiDung`.
- `LichLamViec`: lịch làm việc của bác sĩ theo thứ và ca.
- `BenhNhan`: thông tin bệnh nhân, liên kết 1-1 với `NguoiDung`.
- `DangKyLichKham`: lịch hẹn khám của bệnh nhân.
- `PhieuKham`: phiếu khám được lập từ lịch hẹn.
- `ChiTietDichVuKham`: dịch vụ được chỉ định trong phiếu khám.
- `Thuoc`: danh mục thuốc.
- `DonThuoc`: đơn thuốc theo phiếu khám.
- `ChiTietDonThuoc`: thuốc trong đơn thuốc.
- `HoaDon`: hóa đơn thanh toán của bệnh nhân.
- `ChiTietHoaDon`: chi tiết tiền khám/tiền thuốc theo phiếu khám và đơn thuốc.

## Luồng nghiệp vụ trọng tâm

### Đặt lịch khám

1. Bệnh nhân đăng nhập.
2. Chọn chuyên khoa, bác sĩ, ngày khám và ca khám.
3. Hệ thống kiểm tra lịch làm việc của bác sĩ.
4. Hệ thống kiểm tra sức chứa/trạng thái ca khám.
5. Nếu hợp lệ, tạo bản ghi `DangKyLichKham` với trạng thái `Chờ khám`.
6. Hiển thị xác nhận đặt lịch thành công.

### Khám bệnh

1. Bác sĩ xem lịch khám trong ngày.
2. Bác sĩ tiếp nhận bệnh nhân.
3. Hệ thống chuyển lịch hẹn sang trạng thái `Đang khám` nếu cần.
4. Bác sĩ lập `PhieuKham` với triệu chứng, chẩn đoán, hướng điều trị.
5. Bác sĩ có thể chỉ định `DichVuKham` và/hoặc kê `DonThuoc`.
6. Khi hoàn tất, cập nhật `PhieuKham.TrangThai = Hoàn thành` và `DangKyLichKham.TrangThai = Đã khám`.

### Thanh toán

1. Hệ thống tổng hợp chi phí từ `ChiTietDichVuKham` và `ChiTietDonThuoc`.
2. Tạo `HoaDon` cho bệnh nhân.
3. Tạo `ChiTietHoaDon` liên kết phiếu khám và đơn thuốc.
4. Bệnh nhân thanh toán bằng một trong các hình thức hợp lệ.
5. Cập nhật `HoaDon.TrangThai = Đã thanh toán`.

## Quy ước trạng thái cần giữ nguyên

- `NguoiDung.VaiTro`: `Quản trị`, `Bệnh nhân`, `Bác sĩ`.
- `DangKyLichKham.TrangThai`: `Chờ khám`, `Đang khám`, `Đã khám`, `Hủy`.
- `PhieuKham.TrangThai`: `Đang xử lý`, `Hoàn thành`, `Hủy`.
- `DonThuoc.TrangThai`: `Chờ cấp thuốc`, `Đã cấp thuốc`, `Hủy`.
- `HoaDon.TrangThai`: `Chưa thanh toán`, `Đã thanh toán`, `Đã hủy`.
- Ca khám/làm việc: `Sáng`, `Chiều`, `Tối`.
- Ngày trong tuần: `Thứ 2`, `Thứ 3`, `Thứ 4`, `Thứ 5`, `Thứ 6`, `Thứ 7`, `Chủ nhật`.

## Ghi chú kỹ thuật quan trọng

- Không hard-code connection string hoặc mật khẩu trong source code.
- `ApplicationDbContext.cs` hiện đang có connection string scaffold trực tiếp. Việc nên làm sớm: chuyển sang `builder.Configuration.GetConnectionString(...)` trong `Program.cs` và lưu secret ở `appsettings.Development.json` hoặc User Secrets.
- Không commit `appsettings.Development.json`, `.env`, `.vs/`, `bin/`, `obj/`, `*.user`.
- Các entity hiện được scaffold từ database. Khi sửa schema, ưu tiên cập nhật SQL script và scaffold/migration nhất quán.
- Mã khóa chính đang dùng `VARCHAR(10)`, ví dụ `ND001`, `BS001`, `BN001`, `DK001`. Khi thêm dữ liệu cần giữ format thống nhất.

## Ghi nhớ giao diện hiện tại

- Login nằm ở `Views/Account/Login.cshtml`, dùng ảnh minh họa `wwwroot/images/br.png`.
- Trang chủ theo vai trò nằm trong `Views/Dashboard/`: `BenhNhan.cshtml`, `BacSi.cshtml`, `Admin.cshtml`.
- Các dashboard đang dùng phong cách web app với class gốc `role-page web-dashboard`.
- CSS chính cho dashboard nằm cuối `wwwroot/css/site.css`, phần `/* Web dashboard refresh */`.
- JavaScript dùng chung cho dashboard nằm ở `wwwroot/js/dashboard.js`.
- Đăng xuất không dùng `confirm()` mặc định của trình duyệt. Các form logout dùng class `logout-form` và hiển thị modal custom `.app-confirm`.
- Font giao diện dashboard dùng hệ font bình thường: `"Segoe UI", Arial, Helvetica, sans-serif`.
- Không thiết kế dashboard theo kiểu WinForms cũ. Ưu tiên sidebar gọn, card thống kê, bảng dữ liệu sạch, màu teal/trắng/xám nhẹ.

## Tài liệu liên quan

- `system_rules.md`: quy tắc bắt buộc khi AI code trong dự án.
- `docs/database_schema.md`: mô tả schema và quan hệ bảng.
- `docs/business_flows.md`: luồng nghiệp vụ từ các sơ đồ.
- `docs/feature_backlog.md`: danh sách module/chức năng nên triển khai.
- `docs/ai_workflow.md`: checklist cho mỗi lần nhờ AI sửa code.
