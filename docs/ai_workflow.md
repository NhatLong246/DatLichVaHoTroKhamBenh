# AI Workflow

## Ghi chú hiện trạng màn hình bác sĩ

Khi sửa tiếp các màn hình bác sĩ, kiểm tra các file hiện có trước:

- `Controllers/DashboardController.cs`: tổng quan bác sĩ lấy dữ liệu thật trong ngày.
- `Controllers/KhamBenhController.cs`: tiếp nhận bệnh nhân, lập phiếu khám, kê đơn, chỉ định dịch vụ, tạo hóa đơn.
- `Controllers/LichLamViecController.cs`: lưới lịch làm việc tuần và danh sách bệnh nhân theo ca.
- `Controllers/CaiDatController.cs`: cài đặt bệnh nhân và bác sĩ.
- `Models/ViewModels/BacSiDashboardViewModel.cs`
- `Models/ViewModels/KhamBenhViewModel.cs`
- `Models/ViewModels/LichLamViecBacSiViewModel.cs`
- `Models/ViewModels/CaiDatViewModel.cs`
- `Views/Dashboard/BacSi.cshtml`
- `Views/KhamBenh/Index.cshtml`
- `Views/LichLamViec/Index.cshtml`
- `Views/CaiDat/BacSi.cshtml`

Quy tắc cần giữ:

- Mọi màn bác sĩ phải lọc theo bác sĩ đang đăng nhập qua `MaNguoiDung` trong session và `BacSi.MaNguoiDung`.
- Không dùng dữ liệu bệnh nhân/số liệu gán cứng trong view.
- Hóa đơn tạo từ khám bệnh hiện chỉ tính tiền dịch vụ/khám; thuốc được lưu trong đơn thuốc riêng và không cộng vào hóa đơn.
- Sidebar bác sĩ dùng các mục `Tổng quan`, `Khám bệnh`, `Lịch làm việc`, `Cài đặt`.
- Các thao tác quan trọng như đăng xuất, đổi mật khẩu, hoàn thành ca khám tiếp tục dùng modal custom, không dùng `confirm()`.

Checklist này dùng mỗi khi nhờ AI code để tiết kiệm token và giảm lỗi.

## Cách bắt đầu một phiên làm việc

Prompt gợi ý:

```text
Đọc MEMORY.md, system_rules.md và tài liệu liên quan trong docs/.
Sau đó triển khai chức năng: <tên chức năng>.
Chỉ sửa các file cần thiết, giữ đúng schema trong btl_table.sql.
Cuối cùng chạy dotnet build và báo cáo file đã sửa.
```

## Checklist trước khi code

- Xác định module: tài khoản, lịch khám, khám bệnh, thanh toán, quản trị.
- Xác định vai trò được phép dùng chức năng.
- Xác định bảng liên quan.
- Xác định trạng thái đầu vào/đầu ra.
- Xác định màn hình cần tạo hoặc sửa.
- Kiểm tra entity và DbContext hiện có.

## Checklist khi sửa database/entity

- Cập nhật `btl_table.sql` nếu thay đổi schema.
- Cập nhật entity/DbContext tương ứng.
- Giữ nguyên constraint tiếng Việt nếu không có lý do đổi.
- Kiểm tra khóa ngoại và cascade delete.
- Không làm lệch model so với database.

## Checklist khi tạo controller/view

- Dùng ViewModel cho form.
- Validate dữ liệu phía server.
- Hiển thị lỗi rõ bằng tiếng Việt.
- POST thành công thì redirect.
- Không để nghiệp vụ phức tạp trong `.cshtml`.
- Với dashboard theo vai trò, giữ cấu trúc web app hiện tại: sidebar, header, metric cards, panel/table.
- Nếu thêm dashboard hoặc trang quản trị mới, dùng lại class/style từ `web-dashboard` thay vì tạo style giống WinForms.
- Nếu cần xác nhận thao tác như đăng xuất/xóa/hủy, dùng modal custom, không dùng `confirm()` mặc định.
- Kiểm tra chữ tiếng Việt trong file `.cshtml` hiển thị đúng UTF-8, không để lỗi mojibake như `BÃ¡c sÄ©`.

## Checklist sau khi code

- Chạy `dotnet build`.
- Kiểm tra warning/error quan trọng.
- Kiểm tra `.gitignore` không bỏ sót file build/secret.
- Tóm tắt:
  - File đã sửa.
  - Chức năng đã làm.
  - Lệnh đã chạy.
  - Việc còn lại hoặc rủi ro.

## Prompt mẫu theo chức năng

### Đặt lịch khám

```text
Hãy triển khai module đặt lịch khám cho bệnh nhân.
Đọc docs/business_flows.md phần "Luồng đặt lịch khám".
Yêu cầu: chọn chuyên khoa, bác sĩ, ngày, ca; kiểm tra LichLamViec; tạo DangKyLichKham trạng thái Chờ khám.
```

Ghi chú hiện trạng: trang đặt lịch đã có `LichKhamController`, `DatLichKhamViewModel`, `Views/LichKham/DatLich.cshtml`, gợi ý chuyên khoa rule-based, chọn khung giờ/thời lượng và lưu `GioKham`, `ThoiLuongKham` vào `DangKyLichKham`. Nếu làm tiếp module này, trước tiên kiểm tra `btl_table.sql`, `Models/Entities/DangKyLichKham.cs`, `sql_update_add_gio_kham_dang_ky.sql` và controller hiện có.

### Lập phiếu khám

```text
Hãy triển khai chức năng bác sĩ lập phiếu khám.
Đọc docs/business_flows.md phần "Luồng khám bệnh".
Yêu cầu: chỉ bác sĩ được thao tác, không lập phiếu cho lịch Hủy, cập nhật trạng thái phù hợp.
```

### Thanh toán

```text
Hãy triển khai chức năng tạo hóa đơn.
Đọc docs/business_flows.md phần "Luồng thanh toán".
Yêu cầu: tính tiền từ ChiTietDichVuKham và ChiTietDonThuoc, tạo HoaDon và ChiTietHoaDon.
```

## Ghi chú hiện trạng các màn hình bệnh nhân

Khi sửa tiếp các màn hình bệnh nhân, kiểm tra các file hiện có trước:

- `Controllers/LichKhamController.cs`: đặt lịch, quản lý lịch hẹn, hủy lịch hẹn.
- `Controllers/HoSoBenhAnController.cs`: xem hồ sơ bệnh án của bệnh nhân.
- `Controllers/HoaDonController.cs`: xem hóa đơn và xác nhận thanh toán.
- `Controllers/CaiDatController.cs`: thông tin cá nhân, thông tin tài khoản, đổi mật khẩu.
- `Models/ViewModels/DatLichKhamViewModel.cs`
- `Models/ViewModels/QuanLyLichHenViewModel.cs`
- `Models/ViewModels/HoSoBenhAnViewModel.cs`
- `Models/ViewModels/HoaDonViewModel.cs`
- `Models/ViewModels/CaiDatViewModel.cs`
- `Views/LichKham/DatLich.cshtml`
- `Views/LichKham/QuanLy.cshtml`
- `Views/HoSoBenhAn/Index.cshtml`
- `Views/HoaDon/Index.cshtml`
- `Views/CaiDat/Index.cshtml`

Quy tắc cần giữ:

- Bệnh nhân chỉ được xem/cập nhật dữ liệu thuộc chính mình, đối chiếu qua `MaNguoiDung` trong session và `BenhNhan.MaNguoiDung`.
- Các thao tác quan trọng như đăng xuất, hủy lịch, thanh toán, đổi mật khẩu dùng modal custom trong `wwwroot/js/dashboard.js`.
- Không dùng `confirm()` mặc định của trình duyệt.
- Trạng thái phải giữ đúng schema: `Chờ khám`, `Hủy`, `Chưa thanh toán`, `Đã thanh toán`, ...
- Trang bệnh nhân dùng cùng sidebar và class `role-page web-dashboard patient-theme`.
- Với dữ liệu render bằng JavaScript từ JSON, cần escape HTML trước khi đưa vào `innerHTML`.
