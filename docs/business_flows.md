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

## Cập nhật luồng đặt lịch khám trực tuyến

Trang đặt lịch hiện tại có thêm bước hỗ trợ bệnh nhân chưa biết nên khám chuyên khoa nào:

1. Bệnh nhân nhập triệu chứng.
2. Hệ thống gọi **Gemini AI API** để phân tích và gợi ý 1-3 chuyên khoa phù hợp cùng thời lượng khám dự kiến.
3. Bệnh nhân có thể dùng gợi ý để tự chọn chuyên khoa, sau đó chọn bác sĩ.
4. Bệnh nhân xem lịch làm việc của bác sĩ và chọn ngày/ca.
5. Hệ thống gọi API `LayThongTinCaKham` bằng AJAX. API sẽ sinh các khung giờ theo thời lượng khám dự kiến, tự động loại bỏ các giờ đã bị trùng với bệnh nhân khác (overlap) và kiểm tra xem ca khám có đạt mức tối đa sức chứa (`SucChua`) hay chưa.
6. Trước khi xác nhận, bệnh nhân xem lại chuyên khoa, bác sĩ, ngày, ca, giờ, thời lượng và phòng khám.
7. Khi POST đặt lịch, server kiểm tra lại lần nữa: lịch làm việc, phòng khám, sức chứa, ngày không quá khứ, giờ nằm trong ca và không trùng giờ với lịch khác của cùng bác sĩ.
8. Nếu hợp lệ, tạo `DangKyLichKham` trạng thái `Chờ khám`, có lưu `GioKham` và `ThoiLuongKham`.

Gợi ý chuyên khoa chỉ hỗ trợ điều hướng bệnh nhân đến khoa phù hợp, không phải chẩn đoán y khoa.

## Cập nhật luồng màn hình bệnh nhân

### Quản lý lịch hẹn

1. Bệnh nhân đăng nhập và chọn `Lịch hẹn`.
2. Hệ thống lấy `BenhNhan` theo `MaNguoiDung` trong session.
3. Hệ thống truy vấn `DangKyLichKham` của đúng bệnh nhân, kèm `BacSi`, `ChuyenKhoa`, `PhongKham`, `PhieuKham`.
4. Giao diện hiển thị toàn bộ lịch hẹn và thống kê nhanh theo trạng thái.
5. Nếu lịch ở trạng thái `Chờ khám`, chưa tới thời gian khám và chưa có phiếu khám, hệ thống hiển thị nút `Hủy`.
6. Khi bệnh nhân xác nhận hủy bằng modal custom, hệ thống cập nhật `DangKyLichKham.TrangThai = Hủy`.

### Hồ sơ bệnh án

1. Bệnh nhân chọn `Hồ sơ bệnh án`.
2. Hệ thống chỉ lấy các lịch khám của bệnh nhân hiện tại đã có `PhieuKham`.
3. Giao diện hiển thị danh sách lần khám.
4. Khi chọn một lần khám, phần chi tiết hiển thị bác sĩ, chuyên khoa, phòng khám, triệu chứng, chẩn đoán và hướng điều trị.
5. Tab `Đơn thuốc` hiển thị các `DonThuoc` và chi tiết thuốc từ `ChiTietDonThuoc`.
6. Tab `Dịch vụ đã dùng` hiển thị `ChiTietDichVuKham` và tổng chi phí dịch vụ của lần khám.

### Hóa đơn bệnh nhân

1. Bệnh nhân chọn `Hóa đơn`.
2. Hệ thống lấy `HoaDon` của đúng bệnh nhân hiện tại, kèm `ChiTietHoaDon`, `PhieuKham`, `DangKyLichKham`, bác sĩ, chuyên khoa và phòng khám.
3. Giao diện hiển thị danh sách hóa đơn và trạng thái.
4. Khi chọn một hóa đơn, phần chi tiết hiển thị các dòng tiền khám, tiền thuốc và tổng cộng.
5. Nếu hóa đơn ở trạng thái `Chưa thanh toán`, bệnh nhân chọn hình thức thanh toán hợp lệ và xác nhận.
6. Hệ thống cập nhật `HoaDon.TrangThai = Đã thanh toán` và lưu `HoaDon.HinhThucThanhToan`.

### Cài đặt bệnh nhân

1. Bệnh nhân chọn `Cài đặt`.
2. Hệ thống lấy `BenhNhan` và `NguoiDung` theo session.
3. Giao diện hiển thị thông tin cá nhân và thông tin tài khoản.
4. Các trường định danh như họ tên, ngày sinh, giới tính, mã bệnh nhân, tên đăng nhập, vai trò và trạng thái tài khoản hiển thị chỉ đọc.
5. Bệnh nhân có thể cập nhật số điện thoại và địa chỉ.
6. Bệnh nhân có thể đổi mật khẩu sau khi nhập đúng mật khẩu hiện tại; mật khẩu mới được hash trước khi lưu.
