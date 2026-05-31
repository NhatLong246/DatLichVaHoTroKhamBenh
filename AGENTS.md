# AGENTS

Hướng dẫn cho mọi AI/coding agent làm việc trong repository này.

## Bắt buộc đọc trước khi code

1. `MEMORY.md`
2. `system_rules.md`
3. File liên quan trong `docs/`
4. Code thật trong module sắp sửa

## Nguyên tắc làm việc

- Dự án là ASP.NET Core MVC + EF Core + SQL Server.
- Schema gốc nằm ở `btl_table.sql`.
- Không đoán tên bảng/cột/trạng thái. Luôn đối chiếu database/entity.
- Không hard-code connection string, mật khẩu, token.
- Không commit `appsettings.Development.json`, `.env`, `.vs/`, `bin/`, `obj/`.
- Giữ giao diện và thông báo bằng tiếng Việt.
- Dashboard dùng phong cách web app với class `web-dashboard`; không quay lại kiểu WinForms/mockup desktop cũ.
- Dùng font hệ thống bình thường, ưu tiên `"Segoe UI", Arial, Helvetica, sans-serif`, để chữ tiếng Việt đều và dễ đọc.
- Thao tác đăng xuất/xác nhận dùng modal custom trong `wwwroot/js/dashboard.js`, không dùng hộp thoại mặc định của trình duyệt.
- Sau khi sửa code, chạy `dotnet build` nếu có thể.

## Tài liệu nhanh

- Schema: `docs/database_schema.md`
- Luồng nghiệp vụ: `docs/business_flows.md`
- Backlog module: `docs/feature_backlog.md`
- Checklist làm việc với AI: `docs/ai_workflow.md`

## Ghi chú hiện trạng module đặt lịch

- Trang đặt lịch bệnh nhân đã có tại `Views/LichKham/DatLich.cshtml`, controller `LichKhamController`, ViewModel `DatLichKhamViewModel`.
- Chức năng hiện có: gợi ý chuyên khoa theo triệu chứng bằng **Gemini AI API**, chọn bác sĩ theo chuyên khoa, xem lịch làm việc.
- Việc chọn ngày/ca/giờ hiện sử dụng **Khung giờ khám động (AJAX)** qua API `LayThongTinCaKham`. Hệ thống sẽ chặn các giờ bị trùng (overlap) và kiểm tra quá tải dựa trên sức chứa của phòng khám.
- Schema `DangKyLichKham` đã mở rộng thêm `GioKham` và `ThoiLuongKham`; database cũ cần chạy `sql_update_add_gio_kham_dang_ky.sql`.
- Khi sửa tiếp module này, giữ gợi ý triệu chứng ở mức hỗ trợ chọn chuyên khoa, không viết nội dung như chẩn đoán bệnh.

## Ghi chú hiện trạng các màn hình bệnh nhân

- Tổng quan: `Views/Dashboard/BenhNhan.cshtml`, `DashboardController.BenhNhan`, ViewModel `BenhNhanDashboardViewModel`. Đã dùng dữ liệu thật từ DB.
- Quản lý lịch hẹn: `Views/LichKham/QuanLy.cshtml`, `LichKhamController.QuanLy`, `LichKhamController.HuyLich`, ViewModel `QuanLyLichHenViewModel`.
- Hồ sơ bệnh án: `Views/HoSoBenhAn/Index.cshtml`, `HoSoBenhAnController`, ViewModel `HoSoBenhAnViewModel`.
- Hóa đơn: `Views/HoaDon/Index.cshtml`, `HoaDonController`, ViewModel `HoaDonViewModel`.
- Cài đặt: `Views/CaiDat/Index.cshtml`, `CaiDatController`, ViewModel `CaiDatViewModel`. Giao diện dùng **Bootstrap 5 Card và Floating Labels** hiện đại.
- Sidebar bệnh nhân dùng các mục: `Tổng quan`, `Đặt lịch khám`, `Lịch hẹn`, `Hồ sơ bệnh án`, `Hóa đơn`, `Cài đặt`.
- Các màn hình bệnh nhân phải lọc dữ liệu theo bệnh nhân đang đăng nhập qua `MaNguoiDung` trong session; không cho xem/sửa dữ liệu của bệnh nhân khác.
- Các thao tác quan trọng như hủy lịch, thanh toán, đổi mật khẩu, đăng xuất dùng modal custom trong `wwwroot/js/dashboard.js`.
- Trang cài đặt chỉ cho sửa điện thoại/địa chỉ; họ tên, ngày sinh, giới tính và thông tin tài khoản hiển thị chỉ đọc.

## Ghi chú hiện trạng các màn hình quản trị

- Các module: Báo cáo thống kê, Quản lý Danh mục, Quản lý Lịch, Quản lý Bác sĩ, Quản lý Tài khoản.
- Sidebar menu quản trị đã được hợp nhất thành Partial View `Views/Shared/_AdminSidebar.cshtml`, sử dụng `ViewContext.RouteData` để xác định trạng thái active của menu. Không copy paste `<aside>` ở các file riêng lẻ nữa.
- Các giao diện quản trị sử dụng `admin-theme` với thiết kế sáng sủa, góc bo tròn, màu xanh chủ đạo, có chứa các the metric-card hiển thị chỉ số tổng quát.
