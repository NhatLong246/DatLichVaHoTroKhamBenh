# system_rules - Quy tắc code với AI

File này là quy tắc bắt buộc cho AI khi hỗ trợ code dự án `HeThongDatLichVaKhamBenh`.

## Quy tắc đọc ngữ cảnh

Trước khi sửa code:

1. Đọc `MEMORY.md`.
2. Đọc tài liệu liên quan trong `docs/`.
3. Kiểm tra code hiện tại bằng `rg`/`Get-Content`, không đoán cấu trúc.
4. Chỉ sửa đúng phạm vi yêu cầu.
5. Không xóa/sửa thay đổi không liên quan của người dùng.

## Kiến trúc dự án

- Dùng ASP.NET Core MVC.
- Controllers đặt trong `Controllers/`.
- Entity models đặt trong `Models/Entities/`.
- EF DbContext đặt trong `Models/EF/ApplicationDbContext.cs`.
- Razor views đặt theo convention `Views/{Controller}/{Action}.cshtml`.
- Static files đặt trong `wwwroot/`.

## Quy tắc bảo mật

- Không commit mật khẩu, connection string thật, token, private key.
- Không hard-code secret trong C#.
- Không hiển thị mật khẩu hoặc thông tin nhạy cảm trong view/log.
- Mật khẩu người dùng phải được hash trước khi lưu database. Không lưu plain text.
- Kiểm tra phân quyền theo vai trò trước các chức năng của bác sĩ/quản trị.
- Bệnh nhân chỉ được xem/sửa dữ liệu của chính mình.

## Quy tắc database và EF Core

- Tên bảng/cột phải khớp với `btl_table.sql` và entity hiện có.
- Không đổi trạng thái tiếng Việt nếu không sửa cả CHECK constraint trong SQL.
- Khi truy vấn dữ liệu có navigation, dùng `Include` rõ ràng.
- Với thao tác tạo lịch khám, phải kiểm tra:
  - Bác sĩ có lịch làm việc đúng ngày/ca.
  - Phòng khám còn hoạt động.
  - Ngày khám không ở quá khứ.
  - Ca khám thuộc `Sáng`, `Chiều`, `Tối`.
  - Không vượt quá sức chứa/phòng hoặc quy tắc slot được thiết kế.
- Với thanh toán, tính tiền từ chi tiết dịch vụ và chi tiết thuốc, không nhập tay tổng tiền nếu có thể tính tự động.

## Quy tắc MVC

- Controller chỉ điều phối request, validate, gọi service/repository nếu đã có.
- Không nhồi quá nhiều nghiệp vụ vào Razor view.
- ViewModel nên dùng cho form và màn hình tổng hợp, không truyền entity phức tạp trực tiếp nếu màn hình cần dữ liệu từ nhiều bảng.
- Dùng validation ở cả server-side và client-side khi có form.
- Sau POST thành công, dùng redirect theo pattern PRG để tránh submit lại form.

## Quy tắc giao diện

- Giao diện tiếng Việt, dễ hiểu cho bệnh nhân/bác sĩ/quản trị.
- Ưu tiên Bootstrap sẵn có trong `wwwroot/lib/bootstrap`.
- Form đặt lịch phải rõ các bước: chuyên khoa, bác sĩ, ngày, ca, xác nhận.
- Trang bác sĩ cần ưu tiên tốc độ thao tác: danh sách bệnh nhân, trạng thái lịch, nút tiếp nhận/lập phiếu.
- Trang quản trị cần bảng dữ liệu có tìm kiếm/lọc cơ bản nếu danh sách dài.
- Dashboard cho 3 vai trò phải giữ phong cách web app, không mô phỏng WinForms.
- Dashboard dùng class nền `web-dashboard`; các thành phần chính gồm `role-sidebar`, `role-brand`, `metric-grid`, `metric-card`, `table-section`, `data-table`.
- Font dashboard dùng `"Segoe UI", Arial, Helvetica, sans-serif`; không dùng font lạ, font trang trí hoặc font làm dấu tiếng Việt lệch hàng.
- Màu dashboard ưu tiên teal/trắng/xám nhẹ. Tránh gradient gắt, nút neon, bảng màu xanh tím kiểu desktop mockup cũ.
- Không dùng `alert()`/`confirm()` mặc định của trình duyệt cho thao tác quan trọng. Dùng modal custom đồng bộ giao diện, hiện tại là `.app-confirm` trong `wwwroot/js/dashboard.js` và `wwwroot/css/site.css`.

## Quy tắc đặt tên

- Giữ tên entity theo database: `NguoiDung`, `BacSi`, `BenhNhan`, `DangKyLichKham`, ...
- Controller đặt tên theo module: `LichKhamController`, `KhamBenhController`, `ThanhToanController`, `QuanTriController`.
- ViewModel đặt hậu tố `ViewModel`, ví dụ `DatLichKhamViewModel`.
- Tên biến C# dùng PascalCase cho property, camelCase cho local variable.

## Quy tắc kiểm thử và xác minh

Sau khi sửa code:

1. Chạy `dotnet build`.
2. Nếu có test thì chạy test.
3. Nếu sửa UI, chạy web và kiểm tra flow chính bằng browser.
4. Kiểm tra không phát sinh file build/cache cần commit.
5. Tóm tắt file đã sửa và lệnh đã chạy.

## Việc nên ưu tiên sớm

- Di chuyển connection string ra khỏi `ApplicationDbContext.OnConfiguring`.
- Đăng ký `ApplicationDbContext` trong `Program.cs`.
- Thiết kế authentication/authorization rõ ràng.
- Thêm ViewModel cho đăng nhập, đăng ký, đặt lịch, lập phiếu khám, hóa đơn.
- Tạo seed data mẫu cho chuyên khoa, phòng khám, bác sĩ, lịch làm việc, dịch vụ, thuốc.

## Quy tắc riêng cho module đặt lịch khám

- Trang đặt lịch bệnh nhân hiện dùng `LichKhamController`, `DatLichKhamViewModel` và view `Views/LichKham/DatLich.cshtml`.
- Không bỏ qua kiểm tra server-side khi tạo `DangKyLichKham`; không chỉ dựa vào JavaScript ở view.
- Khi tạo lịch mới phải lưu `GioKham` và `ThoiLuongKham`.
- Khung giờ phải nằm trong ca đã chọn: `Sáng` 07:30-11:30, `Chiều` 13:30-17:00, `Tối` 18:00-20:30.
- `ThoiLuongKham` hợp lệ từ 15 đến 180 phút.
- Phải kiểm tra trùng giờ của cùng bác sĩ trong cùng ngày/ca, tính theo khoảng thời gian bắt đầu `GioKham` và kết thúc `GioKham + ThoiLuongKham`.
- Nếu database hiện có chưa có `DangKyLichKham.GioKham` và `DangKyLichKham.ThoiLuongKham`, phải chạy `sql_update_add_gio_kham_dang_ky.sql` trước khi test đặt lịch thật.
- Gợi ý chuyên khoa bằng triệu chứng chỉ là gợi ý chọn khoa, không phải chẩn đoán. Nội dung UI và thông báo không được khẳng định bệnh.
- Nếu nâng cấp sang AI API, không lưu API key trong source code và phải đối chiếu kết quả AI với dữ liệu `ChuyenKhoa` thật trong database.
