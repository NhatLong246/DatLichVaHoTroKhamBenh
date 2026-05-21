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

## Cập nhật module đặt lịch khám

- Trang đặt lịch khám cho bệnh nhân đã được triển khai tại `Views/LichKham/DatLich.cshtml`.
- Controller chính: `Controllers/LichKhamController.cs`.
- ViewModel chính: `Models/ViewModels/DatLichKhamViewModel.cs`.
- Entity lịch hẹn: `Models/Entities/DangKyLichKham.cs`.
- Link từ dashboard bệnh nhân sang trang đặt lịch đã được nối trong `Views/Dashboard/BenhNhan.cshtml`.
- CSS cho giao diện đặt lịch nằm trong `wwwroot/css/site.css`, dùng chung phong cách `web-dashboard`.
- Trang đặt lịch hiện có: nhập triệu chứng để gợi ý chuyên khoa, chọn bác sĩ theo chuyên khoa, chọn lịch làm việc/ngày/ca/giờ/thời lượng và xác nhận thông tin.
- Gợi ý chuyên khoa đang dùng rule-based trong `LichKhamController.GoiYChuyenKhoa`, chưa gọi AI/API ngoài.
- Rule-based hỗ trợ các nhóm khoa: `Nội khoa`, `Tim mạch`, `Tiêu hóa`, `Tai Mũi Họng`, `Mắt`, `Da liễu`, `Thần kinh`, `Sản phụ khoa`, `Nhi khoa`, `Ngoại khoa`.
- Gợi ý chỉ hỗ trợ chọn chuyên khoa, không được trình bày như chẩn đoán bệnh. Nếu tích hợp AI API sau này, vẫn phải đối chiếu kết quả với bảng `ChuyenKhoa`.
- `DangKyLichKham` đã mở rộng thêm `GioKham TIME(0)` và `ThoiLuongKham INT`.
- Script nâng cấp database hiện có: `sql_update_add_gio_kham_dang_ky.sql`. Cần chạy script này nếu database đã được tạo từ schema cũ.
- Khi đặt lịch phải kiểm tra: đúng vai trò bệnh nhân, bác sĩ thuộc chuyên khoa, có `LichLamViec` đúng thứ/ca, phòng khám hoạt động, ngày không quá khứ, ca hợp lệ, giờ nằm trong ca, thời lượng 15-180 phút, không trùng giờ với lịch khác của cùng bác sĩ.

## Cập nhật các màn hình bệnh nhân

Các màn hình bệnh nhân đã được nối từ sidebar theo phong cách `web-dashboard`, dùng chung CSS trong `wwwroot/css/site.css` và modal xác nhận custom trong `wwwroot/js/dashboard.js`.

### Quản lý lịch hẹn

- Trang quản lý lịch hẹn nằm tại `Views/LichKham/QuanLy.cshtml`.
- Controller/action chính: `LichKhamController.QuanLy` và `LichKhamController.HuyLich`.
- ViewModel: `Models/ViewModels/QuanLyLichHenViewModel.cs`.
- Chức năng hiện có: hiển thị toàn bộ lịch hẹn của bệnh nhân đang đăng nhập, thông tin bác sĩ/chuyên khoa/phòng khám/ngày/ca/giờ/thời lượng/trạng thái, phiếu khám nếu có.
- Bệnh nhân chỉ được hủy lịch của chính mình khi lịch ở trạng thái `Chờ khám`, chưa tới thời gian khám và chưa có `PhieuKham`.
- Hủy lịch cập nhật `DangKyLichKham.TrangThai = Hủy`, dùng modal custom, không dùng `confirm()`.

### Hồ sơ bệnh án

- Trang hồ sơ bệnh án nằm tại `Views/HoSoBenhAn/Index.cshtml`.
- Controller: `Controllers/HoSoBenhAnController.cs`.
- ViewModel: `Models/ViewModels/HoSoBenhAnViewModel.cs`.
- Chức năng hiện có: bệnh nhân xem các lần khám đã có `PhieuKham`, chọn một lần khám để xem chi tiết triệu chứng, chẩn đoán, hướng điều trị, bác sĩ, chuyên khoa, phòng khám.
- Tab `Đơn thuốc` hiển thị `DonThuoc` và bảng `ChiTietDonThuoc` gồm thuốc, số lượng, liều lượng, số ngày dùng, cách dùng, thành tiền.
- Tab `Dịch vụ đã dùng` hiển thị `ChiTietDichVuKham`, tên dịch vụ, mô tả, số lượng, đơn giá, thành tiền và tổng chi phí dịch vụ.
- Dữ liệu render bằng JavaScript từ JSON đã có escape HTML ở phía client để tránh chèn markup không mong muốn.

### Hóa đơn

- Trang hóa đơn nằm tại `Views/HoaDon/Index.cshtml`.
- Controller: `Controllers/HoaDonController.cs`.
- ViewModel: `Models/ViewModels/HoaDonViewModel.cs`.
- Chức năng hiện có: bệnh nhân xem danh sách hóa đơn của chính mình, chọn hóa đơn để xem chi tiết `ChiTietHoaDon` gồm phiếu khám, đơn thuốc nếu có, tiền khám, tiền thuốc, thành tiền và tổng cộng.
- Với hóa đơn `Chưa thanh toán`, bệnh nhân có thể chọn hình thức `Chuyển khoản`, `Thẻ`, `Tiền mặt`, `Bảo hiểm` rồi xác nhận thanh toán.
- Thanh toán cập nhật `HoaDon.TrangThai = Đã thanh toán` và lưu `HoaDon.HinhThucThanhToan`.
- Không cho thanh toán hóa đơn không thuộc bệnh nhân hiện tại hoặc hóa đơn đã thanh toán/đã hủy.

### Cài đặt bệnh nhân

- Trang cài đặt nằm tại `Views/CaiDat/Index.cshtml`.
- Controller: `Controllers/CaiDatController.cs`.
- ViewModel: `Models/ViewModels/CaiDatViewModel.cs`.
- Sidebar bệnh nhân đã đổi mục `Đổi mật khẩu` thành `Cài đặt`.
- Trang cài đặt gồm thông tin cá nhân, thông tin tài khoản và đổi mật khẩu.
- Các trường định danh như mã bệnh nhân, họ tên, ngày sinh, giới tính, mã người dùng, tên đăng nhập, vai trò, trạng thái tài khoản hiển thị `readonly`.
- Bệnh nhân hiện chỉ cập nhật số điện thoại và địa chỉ.
- Đổi mật khẩu yêu cầu mật khẩu hiện tại, mật khẩu mới và xác nhận mật khẩu mới; mật khẩu được hash SHA-256 giống `AccountController`.
