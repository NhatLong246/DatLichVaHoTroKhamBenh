# Database Schema

Nguồn chính: `btl_table.sql`.

## Tổng quan quan hệ

- `NguoiDung` liên kết 1-1 với `BenhNhan` hoặc `BacSi`.
- `ChuyenKhoa` có nhiều `BacSi`, `PhongKham`, `DichVuKham`.
- `BacSi` có nhiều `LichLamViec` và nhiều `DangKyLichKham`.
- `BenhNhan` có nhiều `DangKyLichKham` và nhiều `HoaDon`.
- `DangKyLichKham` có nhiều `PhieuKham`.
- `PhieuKham` có nhiều dịch vụ khám, nhiều đơn thuốc và nhiều chi tiết hóa đơn.
- `DonThuoc` có nhiều `ChiTietDonThuoc`.
- `HoaDon` có nhiều `ChiTietHoaDon`.

## Bảng và ý nghĩa

### NguoiDung

Lưu tài khoản đăng nhập.

- Khóa chính: `MaNguoiDung`.
- Unique: `TenDangNhap`.
- Vai trò hợp lệ: `Quản trị`, `Bệnh nhân`, `Bác sĩ`.
- `TrangThai`: `1` hoạt động, `0` bị khóa/ngừng sử dụng.

### ChuyenKhoa

Danh mục chuyên khoa.

- Khóa chính: `MaChuyenKhoa`.
- `TenChuyenKhoa` bắt buộc.

### PhongKham

Phòng khám thuộc chuyên khoa.

- Khóa chính: `MaPhongKham`.
- FK: `MaChuyenKhoa -> ChuyenKhoa`.
- `SucChua > 0`.
- Trạng thái: `Hoạt động`, `Tạm dừng`, `Bảo trì`.

### DichVuKham

Dịch vụ khám/cận lâm sàng thuộc chuyên khoa.

- Khóa chính: `MaDichVu`.
- FK: `MaChuyenKhoa -> ChuyenKhoa`.
- `GiaTien > 0`.
- `ThoiGianTrungBinh > 0`.
- Trạng thái: `Hoạt động`, `Ngừng cung cấp`.

### BacSi

Thông tin bác sĩ.

- Khóa chính: `MaBacSi`.
- FK unique: `MaNguoiDung -> NguoiDung`.
- FK: `MaChuyenKhoa -> ChuyenKhoa`.
- Giới tính: `Nam`, `Nữ`.
- Tuổi hợp lệ theo SQL hiện tại: 22 đến 70.
- Số điện thoại: 10 chữ số.
- Trình độ: `Bác sĩ`, `Thạc sĩ`, `Tiến sĩ`, `Phó giáo sư`, `Giáo sư`.

### LichLamViec

Lịch làm việc cố định của bác sĩ.

- Khóa chính ghép: `MaBacSi`, `NgayTrongTuan`, `CaLamViec`.
- FK: `MaBacSi -> BacSi`.
- FK: `MaPhongKham -> PhongKham`.
- Ngày trong tuần: `Thứ 2` đến `Chủ nhật`.
- Ca: `Sáng`, `Chiều`, `Tối`.

### BenhNhan

Thông tin bệnh nhân.

- Khóa chính: `MaBenhNhan`.
- FK unique: `MaNguoiDung -> NguoiDung`.
- Giới tính: `Nam`, `Nữ`.
- Ngày sinh không được lớn hơn ngày hiện tại.
- Số điện thoại: 10 chữ số.

### DangKyLichKham

Lịch hẹn khám.

- Khóa chính: `MaDangKy`.
- FK: `MaBacSi -> BacSi`.
- FK: `MaBenhNhan -> BenhNhan`.
- FK: `MaPhongKham -> PhongKham`.
- Ngày khám không được ở quá khứ.
- Ca khám: `Sáng`, `Chiều`, `Tối`.
- Trạng thái: `Chờ khám`, `Đang khám`, `Đã khám`, `Hủy`.

### PhieuKham

Phiếu khám được lập từ lịch hẹn.

- Khóa chính: `MaPhieuKham`.
- FK: `MaDangKy -> DangKyLichKham`.
- Lưu `TrieuChung`, `ChanDoan`, `HuongDieuTri`.
- Trạng thái: `Đang xử lý`, `Hoàn thành`, `Hủy`.

### ChiTietDichVuKham

Dịch vụ được chỉ định trong phiếu khám.

- Khóa chính ghép: `MaPhieuKham`, `MaDichVu`.
- FK: `MaPhieuKham -> PhieuKham`.
- FK: `MaDichVu -> DichVuKham`.
- `ThanhTien` là computed column: `SoLuong * DonGia`.

### Thuoc

Danh mục thuốc.

- Khóa chính: `MaThuoc`.
- `SoLuongTon >= 0`.
- `GiaBan > 0`.

### DonThuoc

Đơn thuốc theo phiếu khám.

- Khóa chính: `MaDonThuoc`.
- FK: `MaPhieuKham -> PhieuKham`.
- Trạng thái: `Chờ cấp thuốc`, `Đã cấp thuốc`, `Hủy`.

### ChiTietDonThuoc

Chi tiết thuốc trong đơn.

- Khóa chính ghép: `MaDonThuoc`, `MaThuoc`.
- FK: `MaDonThuoc -> DonThuoc`.
- FK: `MaThuoc -> Thuoc`.
- `ThanhTien` là computed column: `SoLuong * DonGia`.

### HoaDon

Hóa đơn thanh toán.

- Khóa chính: `MaHoaDon`.
- FK: `MaBenhNhan -> BenhNhan`.
- Hình thức thanh toán: `Tiền mặt`, `Thẻ`, `Chuyển khoản`, `Bảo hiểm`.
- Trạng thái: `Chưa thanh toán`, `Đã thanh toán`, `Đã hủy`.

### ChiTietHoaDon

Chi tiết tiền khám và tiền thuốc.

- Khóa chính: `MaChiTiet` identity.
- FK: `MaHoaDon -> HoaDon`.
- FK: `MaPhieuKham -> PhieuKham`.
- FK nullable: `MaDonThuoc -> DonThuoc`.
- `TienKham >= 0`, `TienThuoc >= 0`.

## Gợi ý truy vấn phổ biến

- Lịch khám của bệnh nhân: `DangKyLichKham` include `BacSi`, `PhongKham`, `PhieuKham`.
- Lịch làm việc bác sĩ: `LichLamViec` include `PhongKham`, `BacSi`, `ChuyenKhoa`.
- Danh sách bệnh nhân theo bác sĩ/ngày: `DangKyLichKham` filter `MaBacSi`, `NgayKham`, `CaKham`, trạng thái khác `Hủy`.
- Chi phí phiếu khám: tổng `ChiTietDichVuKham.ThanhTien` + tổng `ChiTietDonThuoc.ThanhTien`.
