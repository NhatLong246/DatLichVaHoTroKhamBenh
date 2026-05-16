# Business Flows

Tài liệu này tóm tắt các sơ đồ chức năng, ngữ cảnh, luồng dữ liệu, use case, activity và sequence đã thiết kế.

## Phân rã chức năng

### 1. Quản lý người dùng

- Đăng ký và đăng nhập.
- Cập nhật thông tin cá nhân.
- Phân quyền sử dụng.

### 2. Quản lý lịch khám

- Đặt lịch khám.
- Điều chỉnh lịch hẹn.
- Quản lý lịch làm việc bác sĩ.
- Theo dõi tình trạng lịch hẹn.

### 3. Quản lý khám bệnh

- Lập phiếu khám.
- Kê đơn thuốc.
- Cập nhật hồ sơ bệnh án.
- Xem kết quả khám.

### 4. Quản lý thanh toán

- Tạo hóa đơn thanh toán.
- Xem lịch sử thanh toán.
- Báo cáo doanh thu.

### 5. Quản trị hệ thống

- Quản lý bác sĩ.
- Quản lý dữ liệu bệnh nhân.
- Báo cáo tổng hợp.

## Tác nhân ngoài hệ thống

- `Bệnh nhân`: gửi yêu cầu đăng ký/đăng nhập, đặt lịch, thay đổi lịch, nhận thông báo xác nhận, nhận kết quả khám/đơn thuốc/hóa đơn.
- `Bác sĩ`: nhận lịch khám trong ngày, cập nhật hồ sơ bệnh án, lập phiếu khám, kê đơn thuốc.
- `Quản trị viên`: cấu hình hệ thống, quản lý dữ liệu, nhận báo cáo thống kê.

## Use case chính

### Bệnh nhân

- Quản lý lịch hẹn: đặt, hủy, đổi lịch.
- Tra cứu lịch sử: khám, đơn thuốc, thanh toán.
- Quản lý thanh toán/hóa đơn.
- Quản lý tài khoản: đăng nhập, đăng ký, cập nhật thông tin.

### Bác sĩ

- Quản lý khám bệnh: phiếu khám, kê đơn.
- Xem danh sách bệnh nhân và lịch làm việc.
- Quản lý tài khoản cá nhân.

### Quản trị viên

- Thống kê và báo cáo.
- Quản lý hệ thống: người dùng, bác sĩ, khoa/chuyên khoa, phòng khám, dịch vụ, thuốc, lịch làm việc.

## Luồng đặt lịch khám

1. Bệnh nhân chọn chức năng đặt lịch.
2. Web hiển thị form tìm kiếm/chọn bác sĩ.
3. Bệnh nhân nhập chuyên khoa, bác sĩ, ngày và ca khám.
4. Controller kiểm tra lịch làm việc theo bác sĩ, ngày trong tuần và ca.
5. Database trả về tình trạng ca khám.
6. Nếu ca khám đã đầy:
   - Hệ thống báo lỗi `Ca khám đã đầy`.
   - Bệnh nhân chọn thời gian khác.
7. Nếu ca khám hợp lệ:
   - Insert vào `DangKyLichKham`.
   - Tạo thông báo xác nhận.
   - Trả về kết quả đặt lịch thành công.
8. Web hiển thị thông báo thành công và chi tiết lịch.

## Luồng khám bệnh

1. Bệnh nhân đến phòng khám theo lịch.
2. Bác sĩ tiếp nhận bệnh nhân.
3. Bác sĩ thực hiện khám bệnh.
4. Bác sĩ lập phiếu khám, cập nhật chẩn đoán và triệu chứng.
5. Bác sĩ chọn một trong các hướng:
   - Kê đơn thuốc.
   - Chỉ định dịch vụ.
   - Hoàn tất mà không kê đơn/dịch vụ.
6. Bác sĩ hoàn tất phiếu khám.
7. Hệ thống tổng hợp chi phí và lập hóa đơn.
8. Bệnh nhân thanh toán hóa đơn.

## Luồng thanh toán

1. Hệ thống nhận thông tin bệnh nhân và dịch vụ khám từ `PhieuKham`.
2. Tính tiền khám từ `ChiTietDichVuKham`.
3. Tính tiền thuốc từ `ChiTietDonThuoc` nếu có đơn thuốc.
4. Tạo `HoaDon` và `ChiTietHoaDon`.
5. Bệnh nhân thanh toán.
6. Cập nhật trạng thái hóa đơn.

## Luồng quản trị

1. Quản trị viên đăng nhập.
2. Quản lý danh mục nền:
   - Chuyên khoa.
   - Phòng khám.
   - Bác sĩ.
   - Lịch làm việc.
   - Dịch vụ khám.
   - Thuốc.
3. Theo dõi dữ liệu vận hành:
   - Danh sách bệnh nhân.
   - Lịch hẹn.
   - Phiếu khám.
   - Hóa đơn.
4. Xem báo cáo tổng hợp và báo cáo doanh thu.

## Quy tắc nghiệp vụ cần cẩn thận

- Một `NguoiDung` chỉ nên gắn với một hồ sơ `BenhNhan` hoặc `BacSi`.
- Bác sĩ chỉ nhận lịch nếu có `LichLamViec` phù hợp.
- Không cho đặt lịch ở ngày quá khứ.
- Không cho tạo phiếu khám cho lịch đã hủy.
- Không cho thanh toán hóa đơn đã hủy hoặc đã thanh toán.
- Khi hủy lịch đã có phiếu khám/hóa đơn cần xác định chính sách nghiệp vụ trước khi code.
