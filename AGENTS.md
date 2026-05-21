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
- Chức năng hiện có: gợi ý chuyên khoa theo triệu chứng bằng rule-based, chọn bác sĩ theo chuyên khoa, xem lịch làm việc, chọn ngày/ca/giờ/thời lượng, xác nhận thông tin và tạo `DangKyLichKham`.
- Schema `DangKyLichKham` đã mở rộng thêm `GioKham` và `ThoiLuongKham`; database cũ cần chạy `sql_update_add_gio_kham_dang_ky.sql`.
- Khi sửa tiếp module này, giữ gợi ý triệu chứng ở mức hỗ trợ chọn chuyên khoa, không viết nội dung như chẩn đoán bệnh.
