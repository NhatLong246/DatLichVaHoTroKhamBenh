USE HeThongDatLichVaKhamBenh
GO

-- ============================================
-- 1. THÊM DỮ LIỆU NGƯỜI DÙNG (với mật khẩu đã băm SHA-256)
-- ============================================
-- Mật khẩu gốc: Admin@123 -> Hash SHA-256
-- Mật khẩu gốc: Bs@12345 -> Hash SHA-256
-- Mật khẩu gốc: Bn@12345 -> Hash SHA-256
-- 1. Người dùng
SELECT * FROM NguoiDung;

-- 2. Chuyên khoa
SELECT * FROM ChuyenKhoa;

-- 3. Phòng khám
SELECT * FROM PhongKham;

-- 4. Dịch vụ khám
SELECT * FROM DichVuKham;

-- 5. Bác sĩ
SELECT * FROM BacSi;

-- 6. Lịch làm việc
SELECT * FROM LichLamViec;

-- 7. Bệnh nhân
SELECT * FROM BenhNhan;

-- 8. Đăng ký lịch khám
SELECT * FROM DangKyLichKham;

-- 9. Phiếu khám
SELECT * FROM PhieuKham;

-- 10. Chi tiết dịch vụ khám
SELECT * FROM ChiTietDichVuKham;

-- 11. Thuốc
SELECT * FROM Thuoc;

-- 12. Đơn thuốc
SELECT * FROM DonThuoc;

-- 13. Chi tiết đơn thuốc
SELECT * FROM ChiTietDonThuoc;

-- 14. Hóa đơn
SELECT * FROM HoaDon;

-- 15. Chi tiết hóa đơn
SELECT * FROM ChiTietHoaDon;


INSERT INTO NguoiDung (MaNguoiDung, TenDangNhap, MatKhau, VaiTro, TrangThai, Email) VALUES
('ND001', N'admin01', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Admin@123'), 2), N'Quản trị', 1, 'admin@healthcare.com'),
('ND002', N'bsnguyenvana', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bs@12345'), 2), N'Bác sĩ', 1, 'nguyenvana@healthcare.com'),
('ND003', N'bstranthib', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bs@12345'), 2), N'Bác sĩ', 1, 'tranthib@healthcare.com'),
('ND004', N'bslevanc', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bs@12345'), 2), N'Bác sĩ', 1, 'levanc@healthcare.com'),
('ND005', N'bsphamthid', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bs@12345'), 2), N'Bác sĩ', 1, 'phamthid@healthcare.com'),
('ND006', N'bshoangvane', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bs@12345'), 2), N'Bác sĩ', 1, 'hoangvane@healthcare.com'),
('ND007', N'bn.nguyenminh', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'nguyenminh@gmail.com'),
('ND008', N'bn.tranhong', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'tranhong@gmail.com'),
('ND009', N'bn.levanthanh', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'levanthanh@gmail.com'),
('ND010', N'bn.phamthuhang', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'phamthuhang@gmail.com'),
('ND011', N'bn.hoangvanduc', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'hoangvanduc@gmail.com'),
('ND012', N'bn.ngothilan', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'ngothilan@gmail.com'),
('ND013', N'bn.vuvanbinh', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'vuvanbinh@gmail.com'),
('ND014', N'bn.dothikhanh', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'dothikhanh@gmail.com'),
('ND015', N'bn.buithithu', CONVERT(NVARCHAR(100), HASHBYTES('SHA2_256', N'Bn@12345'), 2), N'Bệnh nhân', 1, 'buithithu@gmail.com');

-- ============================================
-- 2. THÊM DỮ LIỆU CHUYÊN KHOA
-- ============================================
INSERT INTO ChuyenKhoa (MaChuyenKhoa, TenChuyenKhoa) VALUES
('CK01', N'Nội khoa'),
('CK02', N'Ngoại khoa'),
('CK03', N'Sản phụ khoa'),
('CK04', N'Nhi khoa'),
('CK05', N'Tim mạch'),
('CK06', N'Tiêu hóa'),
('CK07', N'Tai Mũi Họng'),
('CK08', N'Mắt'),
('CK09', N'Da liễu'),
('CK10', N'Thần kinh');

-- ============================================
-- 3. THÊM DỮ LIỆU PHÒNG KHÁM
-- ============================================
INSERT INTO PhongKham (MaPhongKham, TenPhongKham, MaChuyenKhoa, ViTri, SucChua, TrangThai, GhiChu) VALUES
('PK01', N'Phòng Nội khoa 1', 'CK01', N'Tầng 1 - Khu A', 20, N'Hoạt động', N'Phòng khám tổng quát'),
('PK02', N'Phòng Nội khoa 2', 'CK01', N'Tầng 1 - Khu A', 15, N'Hoạt động', NULL),
('PK03', N'Phòng Ngoại khoa 1', 'CK02', N'Tầng 2 - Khu B', 10, N'Hoạt động', N'Phòng khám chuyên khoa'),
('PK04', N'Phòng Sản phụ khoa', 'CK03', N'Tầng 3 - Khu C', 15, N'Hoạt động', NULL),
('PK05', N'Phòng Nhi khoa', 'CK04', N'Tầng 1 - Khu D', 25, N'Hoạt động', N'Chuyên khám cho trẻ em'),
('PK06', N'Phòng Tim mạch', 'CK05', N'Tầng 2 - Khu A', 12, N'Hoạt động', NULL),
('PK07', N'Phòng Tiêu hóa', 'CK06', N'Tầng 2 - Khu B', 15, N'Hoạt động', NULL),
('PK08', N'Phòng TMH', 'CK07', N'Tầng 3 - Khu A', 18, N'Hoạt động', NULL),
('PK09', N'Phòng Mắt', 'CK08', N'Tầng 3 - Khu B', 12, N'Bảo trì', N'Đang nâng cấp thiết bị'),
('PK10', N'Phòng Da liễu', 'CK09', N'Tầng 1 - Khu C', 20, N'Hoạt động', NULL);

-- ============================================
-- 4. THÊM DỮ LIỆU DỊCH VỤ KHÁM
-- ============================================
INSERT INTO DichVuKham (MaDichVu, TenDichVu, MoTa, MaChuyenKhoa, GiaTien, ThoiGianTrungBinh, TrangThai) VALUES
('DV001', N'Khám nội khoa tổng quát', N'Khám sức khỏe tổng quát, tư vấn điều trị', 'CK01', 200000, 30, N'Hoạt động'),
('DV002', N'Khám tim mạch', N'Khám chuyên sâu về tim mạch, đo điện tim', 'CK05', 300000, 45, N'Hoạt động'),
('DV003', N'Khám tiêu hóa', N'Khám và tư vấn về các bệnh lý tiêu hóa', 'CK06', 250000, 40, N'Hoạt động'),
('DV004', N'Khám ngoại khoa', N'Khám và điều trị các bệnh ngoại khoa', 'CK02', 250000, 35, N'Hoạt động'),
('DV005', N'Khám sản phụ khoa', N'Khám thai, tư vấn sức khỏe phụ nữ', 'CK03', 300000, 40, N'Hoạt động'),
('DV006', N'Khám nhi khoa', N'Khám và điều trị bệnh cho trẻ em', 'CK04', 200000, 30, N'Hoạt động'),
('DV007', N'Khám tai mũi họng', N'Khám và điều trị các bệnh TMH', 'CK07', 200000, 30, N'Hoạt động'),
('DV008', N'Khám mắt', N'Khám tổng quát về mắt, đo thị lực', 'CK08', 200000, 35, N'Hoạt động'),
('DV009', N'Khám da liễu', N'Khám và điều trị các bệnh về da', 'CK09', 200000, 30, N'Hoạt động'),
('DV010', N'Khám thần kinh', N'Khám và điều trị các bệnh thần kinh', 'CK10', 300000, 45, N'Hoạt động'),
('DV011', N'Siêu âm tim', N'Siêu âm kiểm tra chức năng tim', 'CK05', 500000, 60, N'Hoạt động'),
('DV012', N'Siêu âm thai', N'Siêu âm kiểm tra thai nhi', 'CK03', 400000, 45, N'Hoạt động'),
('DV013', N'Nội soi dạ dày', N'Nội soi kiểm tra dạ dày', 'CK06', 800000, 90, N'Hoạt động'),
('DV014', N'Xét nghiệm máu tổng quát', N'Xét nghiệm các chỉ số máu cơ bản', 'CK01', 150000, 15, N'Hoạt động'),
('DV015', N'Chụp X-quang', N'Chụp X-quang kiểm tra xương khớp', 'CK02', 200000, 20, N'Hoạt động');

-- ============================================
-- 5. THÊM DỮ LIỆU BÁC SĨ
-- ============================================
INSERT INTO BacSi (MaBacSi, MaNguoiDung, HoTen, GioiTinh, NgaySinh, DienThoai, DiaChi, TrinhDo, MaChuyenKhoa, Email) VALUES
('BS001', 'ND002', N'Nguyễn Văn A', N'Nam', '1980-05-15', '0901234567', N'123 Lê Lợi, Q1, TP.HCM', N'Tiến sĩ', 'CK01', 'nguyenvana@healthcare.com'),
('BS002', 'ND003', N'Trần Thị B', N'Nữ', '1985-08-20', '0912345678', N'456 Nguyễn Huệ, Q1, TP.HCM', N'Thạc sĩ', 'CK05', 'tranthib@healthcare.com'),
('BS003', 'ND004', N'Lê Văn C', N'Nam', '1978-12-10', '0923456789', N'789 Hai Bà Trưng, Q3, TP.HCM', N'Giáo sư', 'CK06', 'levanc@healthcare.com'),
('BS004', 'ND005', N'Phạm Thị D', N'Nữ', '1990-03-25', '0934567890', N'321 Trần Hưng Đạo, Q5, TP.HCM', N'Bác sĩ', 'CK03', 'phamthid@healthcare.com'),
('BS005', 'ND006', N'Hoàng Văn E', N'Nam', '1982-07-18', '0945678901', N'654 Lý Thường Kiệt, Q10, TP.HCM', N'Phó giáo sư', 'CK02', 'hoangvane@healthcare.com');

-- ============================================
-- 6. THÊM DỮ LIỆU LỊCH LÀM VIỆC
-- ============================================
INSERT INTO LichLamViec (MaBacSi, NgayTrongTuan, CaLamViec, MaPhongKham) VALUES
-- BS001 - Nội khoa
('BS001', N'Thứ 2', N'Sáng', 'PK01'),
('BS001', N'Thứ 2', N'Chiều', 'PK01'),
('BS001', N'Thứ 3', N'Sáng', 'PK01'),
('BS001', N'Thứ 4', N'Chiều', 'PK02'),
('BS001', N'Thứ 5', N'Sáng', 'PK01'),
('BS001', N'Thứ 6', N'Chiều', 'PK01'),
-- BS002 - Tim mạch
('BS002', N'Thứ 2', N'Sáng', 'PK06'),
('BS002', N'Thứ 3', N'Chiều', 'PK06'),
('BS002', N'Thứ 4', N'Sáng', 'PK06'),
('BS002', N'Thứ 5', N'Chiều', 'PK06'),
('BS002', N'Thứ 6', N'Sáng', 'PK06'),
-- BS003 - Tiêu hóa
('BS003', N'Thứ 2', N'Chiều', 'PK07'),
('BS003', N'Thứ 3', N'Sáng', 'PK07'),
('BS003', N'Thứ 4', N'Chiều', 'PK07'),
('BS003', N'Thứ 5', N'Sáng', 'PK07'),
('BS003', N'Thứ 6', N'Chiều', 'PK07'),
-- BS004 - Sản phụ khoa
('BS004', N'Thứ 2', N'Sáng', 'PK04'),
('BS004', N'Thứ 2', N'Chiều', 'PK04'),
('BS004', N'Thứ 3', N'Sáng', 'PK04'),
('BS004', N'Thứ 4', N'Sáng', 'PK04'),
('BS004', N'Thứ 5', N'Chiều', 'PK04'),
-- BS005 - Ngoại khoa
('BS005', N'Thứ 3', N'Chiều', 'PK03'),
('BS005', N'Thứ 4', N'Sáng', 'PK03'),
('BS005', N'Thứ 5', N'Chiều', 'PK03'),
('BS005', N'Thứ 6', N'Sáng', 'PK03'),
('BS005', N'Thứ 6', N'Chiều', 'PK03');

INSERT INTO LichLamViec (MaBacSi, NgayTrongTuan, CaLamViec, MaPhongKham) VALUES
('BS001', N'Thứ 7', N'Sáng', 'PK01'),   -- Bác sĩ Nội khoa khám sáng thứ 7
('BS002', N'Thứ 7', N'Chiều', 'PK06'),  -- Bác sĩ Tim mạch khám chiều thứ 7
('BS003', N'Thứ 7', N'Sáng', 'PK07');   -- Bác sĩ Tiêu hóa khám sáng thứ 7
-- ============================================
-- 7. THÊM DỮ LIỆU BỆNH NHÂN
-- ============================================
INSERT INTO BenhNhan (MaBenhNhan, MaNguoiDung, HoTen, GioiTinh, NgaySinh, DienThoai, DiaChi) VALUES
('BN001', 'ND007', N'Nguyễn Minh Tuấn', N'Nam', '1992-04-15', '0987654321', N'12 Võ Văn Tần, Q3, TP.HCM'),
('BN002', 'ND008', N'Trần Hồng Nhung', N'Nữ', '1988-11-20', '0976543210', N'45 Điện Biên Phủ, Q1, TP.HCM'),
('BN003', 'ND009', N'Lê Văn Thành', N'Nam', '1975-06-30', '0965432109', N'78 Pasteur, Q1, TP.HCM'),
('BN004', 'ND010', N'Phạm Thu Hằng', N'Nữ', '1995-09-12', '0954321098', N'23 Nguyễn Thị Minh Khai, Q1, TP.HCM'),
('BN005', 'ND011', N'Hoàng Văn Đức', N'Nam', '1980-02-28', '0943210987', N'56 Lê Duẩn, Q1, TP.HCM'),
('BN006', 'ND012', N'Ngô Thị Lan', N'Nữ', '1998-12-05', '0932109876', N'89 Cách Mạng Tháng 8, Q10, TP.HCM'),
('BN007', 'ND013', N'Vũ Văn Bình', N'Nam', '1970-08-18', '0921098765', N'34 Hoàng Văn Thụ, Q.Tân Bình, TP.HCM'),
('BN008', 'ND014', N'Đỗ Thị Khánh', N'Nữ', '2000-05-22', '0910987654', N'67 Trường Chinh, Q.Tân Bình, TP.HCM'),
('BN009', 'ND015', N'Bùi Thị Thu', N'Nữ', '1985-07-14', '0909876543', N'90 Lý Chính Thắng, Q3, TP.HCM');

-- Xóa dữ liệu lỗi nếu đã insert một phần
DELETE FROM ChiTietHoaDon;
DELETE FROM HoaDon;
DELETE FROM ChiTietDonThuoc;
DELETE FROM DonThuoc;
DELETE FROM Thuoc;
DELETE FROM ChiTietDichVuKham;
DELETE FROM PhieuKham;
DELETE FROM DangKyLichKham;

-- 8. Đăng ký lịch khám
INSERT INTO DangKyLichKham (MaDangKy, MaBacSi, MaBenhNhan, MaPhongKham, NgayKham, CaKham, TrangThai) VALUES
('DK001', 'BS001', 'BN001', 'PK01', '2026-11-06', N'Sáng', N'Chờ khám'),
('DK002', 'BS002', 'BN002', 'PK06', '2026-11-06', N'Sáng', N'Chờ khám'),
('DK003', 'BS003', 'BN003', 'PK07', '2026-11-06', N'Chiều', N'Chờ khám'),
('DK004', 'BS004', 'BN004', 'PK04', '2026-11-07', N'Sáng', N'Chờ khám'),
('DK005', 'BS005', 'BN005', 'PK03', '2026-11-07', N'Chiều', N'Chờ khám'),
('DK006', 'BS001', 'BN006', 'PK01', '2026-11-08', N'Sáng', N'Đã khám'),
('DK007', 'BS002', 'BN007', 'PK06', '2026-11-08', N'Chiều', N'Đã khám'),
('DK008', 'BS003', 'BN008', 'PK07', '2026-11-09', N'Sáng', N'Đã khám'),
('DK009', 'BS001', 'BN009', 'PK01', '2026-11-10', N'Chiều', N'Hủy'),
('DK010', 'BS004', 'BN001', 'PK04', '2026-11-11', N'Sáng', N'Chờ khám'),
('DK012', 'BS002', 'BN002', 'PK01', '2026-11-06', N'Chiều', N'Chờ khám'),
('DK013', 'BS002', 'BN004', 'PK01', '2026-11-06', N'Chiều', N'Chờ khám'),
('DK014', 'BS002', 'BN008', 'PK01', '2026-11-06', N'Chiều', N'Chờ khám'),
('DK015', 'BS001', 'BN003', 'PK01', '2026-11-07', N'Chiều', N'Chờ khám'),
('DK016', 'BS001', 'BN007', 'PK01', '2026-11-07', N'Chiều', N'Chờ khám');

-- 9. Phiếu khám
INSERT INTO PhieuKham (MaPhieuKham, MaDangKy, TrieuChung, ChanDoan, HuongDieuTri, TrangThai) VALUES
('PK001', 'DK006', N'Đau đầu, chóng mặt, mệt mỏi', N'Huyết áp thấp', N'Nghỉ ngơi, bổ sung dinh dưỡng, uống thuốc theo đơn', N'Hoàn thành'),
('PK002', 'DK007', N'Đau ngực, khó thở', N'Loạn nhịp tim nhẹ', N'Uống thuốc điều hòa nhịp tim, tái khám sau 2 tuần', N'Hoàn thành'),
('PK003', 'DK008', N'Đau bụng, buồn nôn', N'Viêm dạ dày cấp', N'Ăn uống điều độ, uống thuốc kháng acid', N'Hoàn thành');

-- 10. Chi tiết dịch vụ khám
INSERT INTO ChiTietDichVuKham (MaPhieuKham, MaDichVu, SoLuong, DonGia) VALUES
('PK001', 'DV001', 1, 200000),
('PK001', 'DV014', 1, 150000),
('PK002', 'DV002', 1, 300000),
('PK002', 'DV011', 1, 500000),
('PK003', 'DV003', 1, 250000),
('PK003', 'DV014', 1, 150000);

-- 11. Thuốc
INSERT INTO Thuoc (MaThuoc, TenThuoc, DonViTinh, SoLuongTon, GiaBan) VALUES
('T001', N'Paracetamol 500mg', N'Viên', 5000, 500),
('T002', N'Amoxicillin 500mg', N'Viên', 3000, 2000),
('T003', N'Vitamin B1', N'Viên', 4000, 1000),
('T004', N'Omeprazole 20mg', N'Viên', 2500, 3000),
('T005', N'Bisoprolol 5mg', N'Viên', 2000, 5000),
('T006', N'Diazepam 5mg', N'Viên', 1500, 3500),
('T007', N'Ibuprofen 400mg', N'Viên', 3500, 1500),
('T008', N'Metformin 500mg', N'Viên', 4000, 2500),
('T009', N'Loratadine 10mg', N'Viên', 3000, 1200),
('T010', N'Cefixime 200mg', N'Viên', 2000, 8000),
('T011', N'Domperidone 10mg', N'Viên', 3000, 1500),
('T012', N'Prednisolone 5mg', N'Viên', 2500, 2000),
('T013', N'Ciprofloxacin 500mg', N'Viên', 2000, 6000),
('T014', N'Dexamethasone 0.5mg', N'Viên', 2000, 1000),
('T015', N'Salbutamol 2mg', N'Viên', 1800, 3000);

-- 12. Đơn thuốc
INSERT INTO DonThuoc (MaDonThuoc, MaPhieuKham, NgayLap, GhiChu, TrangThai) VALUES
('DT001', 'PK001', '2026-11-08', N'Uống thuốc đầy đủ theo chỉ định', N'Đã cấp thuốc'),
('DT002', 'PK002', '2026-11-08', N'Tránh căng thẳng, nghỉ ngơi hợp lý', N'Đã cấp thuốc'),
('DT003', 'PK003', '2026-11-09', N'Ăn uống nhẹ nhàng, tránh cay nóng', N'Đã cấp thuốc');

-- 13. Chi tiết đơn thuốc
INSERT INTO ChiTietDonThuoc (MaDonThuoc, MaThuoc, SoLuong, LieuLuong, SoNgayDung, CachDung, DonGia) VALUES
('DT001', 'T001', 20, N'1 viên/lần', 10, N'Uống 2 lần/ngày sau ăn', 500),
('DT001', 'T003', 30, N'1 viên/lần', 10, N'Uống 3 lần/ngày sau ăn', 1000),
('DT002', 'T005', 28, N'1 viên/lần', 14, N'Uống 2 lần/ngày vào buổi sáng và tối', 5000),
('DT002', 'T007', 30, N'1 viên/lần', 10, N'Uống khi đau, không quá 3 viên/ngày', 1500),
('DT003', 'T004', 28, N'1 viên/lần', 14, N'Uống 2 lần/ngày trước ăn 30 phút', 3000),
('DT003', 'T011', 30, N'1 viên/lần', 10, N'Uống 3 lần/ngày trước ăn 15 phút', 1500);

-- 14. Hóa đơn
INSERT INTO HoaDon (MaHoaDon, MaBenhNhan, NgayLap, TongTien, HinhThucThanhToan, TrangThai) VALUES
('HD001', 'BN006', '2026-11-08', 380000, N'Tiền mặt', N'Đã thanh toán'),
('HD002', 'BN007', '2026-11-08', 950000, N'Thẻ', N'Đã thanh toán'),
('HD003', 'BN008', '2026-11-09', 487000, N'Chuyển khoản', N'Đã thanh toán');

-- 15. Chi tiết hóa đơn
INSERT INTO ChiTietHoaDon (MaHoaDon, MaPhieuKham, MaDonThuoc, TienKham, TienThuoc, GhiChu) VALUES
('HD001', 'PK001', 'DT001', 350000, 30000, N'Thanh toán đầy đủ'),
('HD002', 'PK002', 'DT002', 800000, 150000, N'Thanh toán đầy đủ'),
('HD003', 'PK003', 'DT003', 400000, 87000, N'Thanh toán đầy đủ');


-- ============================================
-- KIỂM TRA DỮ LIỆU ĐÃ THÊM
-- ============================================
PRINT N'=== THỐNG KÊ DỮ LIỆU ĐÃ THÊM ==='
PRINT N'Số người dùng: ' + CAST((SELECT COUNT(*) FROM NguoiDung) AS NVARCHAR(10))
PRINT N'Số chuyên khoa: ' + CAST((SELECT COUNT(*) FROM ChuyenKhoa) AS NVARCHAR(10))
PRINT N'Số phòng khám: ' + CAST((SELECT COUNT(*) FROM PhongKham) AS NVARCHAR(10))
PRINT N'Số dịch vụ khám: ' + CAST((SELECT COUNT(*) FROM DichVuKham) AS NVARCHAR(10))
PRINT N'Số bác sĩ: ' + CAST((SELECT COUNT(*) FROM BacSi) AS NVARCHAR(10))
PRINT N'Số bệnh nhân: ' + CAST((SELECT COUNT(*) FROM BenhNhan) AS NVARCHAR(10))
PRINT N'Số lịch khám: ' + CAST((SELECT COUNT(*) FROM DangKyLichKham) AS NVARCHAR(10))
PRINT N'Số phiếu khám: ' + CAST((SELECT COUNT(*) FROM PhieuKham) AS NVARCHAR(10))
PRINT N'Số loại thuốc: ' + CAST((SELECT COUNT(*) FROM Thuoc) AS NVARCHAR(10))
PRINT N'Số đơn thuốc: ' + CAST((SELECT COUNT(*) FROM DonThuoc) AS NVARCHAR(10))
PRINT N'Số hóa đơn: ' + CAST((SELECT COUNT(*) FROM HoaDon) AS NVARCHAR(10))
PRINT N'=== HOÀN TẤT THÊM DỮ LIỆU MẪU ==='