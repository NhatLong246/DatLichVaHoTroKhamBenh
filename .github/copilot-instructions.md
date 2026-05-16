# Copilot Instructions

Repository này là hệ thống đặt lịch và hỗ trợ khám bệnh viết bằng ASP.NET Core MVC, EF Core và SQL Server.

Trước khi đề xuất hoặc sửa code, hãy đọc:

- `MEMORY.md`
- `system_rules.md`
- `docs/database_schema.md`
- `docs/business_flows.md`

Quy tắc quan trọng:

- Giữ schema khớp với `btl_table.sql`.
- Giữ các trạng thái tiếng Việt đúng constraint SQL.
- Không hard-code connection string hoặc secret.
- Dùng ViewModel cho form và màn hình tổng hợp.
- Không đặt nghiệp vụ phức tạp trong Razor view.
- Kiểm tra phân quyền theo vai trò: `Bệnh nhân`, `Bác sĩ`, `Quản trị`.
- Với đặt lịch khám, luôn kiểm tra lịch làm việc bác sĩ, ngày khám, ca khám và sức chứa/trạng thái phòng khám.
- Với thanh toán, tính tiền từ chi tiết dịch vụ và chi tiết đơn thuốc.
