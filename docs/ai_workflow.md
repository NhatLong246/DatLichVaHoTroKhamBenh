# AI Workflow

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
