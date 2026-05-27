CREATE DATABASE HeThongDatLichVaKhamBenh
GO

USE HeThongDatLichVaKhamBenh
GO

-- 1. Bảng người dùng
CREATE TABLE NguoiDung (
    MaNguoiDung VARCHAR(10) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) UNIQUE,
    MatKhau NVARCHAR(100),
    VaiTro NVARCHAR(20) CHECK (VaiTro IN (N'Quản trị', N'Bệnh nhân', N'Bác sĩ')),
	TrangThai BIT DEFAULT 1, -- 1: hoạt động, 0: bị khóa / ngừng sử dụng
    Email VARCHAR(100)
);

-- 2. Bảng Chuyên Khoa
CREATE TABLE ChuyenKhoa (
    MaChuyenKhoa VARCHAR(10) PRIMARY KEY,
    TenChuyenKhoa NVARCHAR(100) NOT NULL,
    CONSTRAINT CHK_MaChuyenKhoa_Length CHECK (LEN(MaChuyenKhoa) >= 2)
);

-- 3. Bảng Phòng Khám (thêm sau bảng ChuyenKhoa)
CREATE TABLE PhongKham (
    MaPhongKham VARCHAR(10) PRIMARY KEY,
    TenPhongKham NVARCHAR(100) NOT NULL,
    MaChuyenKhoa VARCHAR(10) NOT NULL,
    ViTri NVARCHAR(200),
    SucChua INT NOT NULL,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    GhiChu NVARCHAR(1000),
    FOREIGN KEY (MaChuyenKhoa) REFERENCES ChuyenKhoa(MaChuyenKhoa) ON UPDATE CASCADE,
    CONSTRAINT CHK_PhongKham_SucChua CHECK (SucChua > 0),
    CONSTRAINT CHK_PhongKham_TrangThai CHECK (TrangThai IN (N'Hoạt động', N'Tạm dừng', N'Bảo trì'))
)
GO

-- 4. Bảng Dịch Vụ Khám
CREATE TABLE DichVuKham (
    MaDichVu VARCHAR(10) PRIMARY KEY,
    TenDichVu NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(1000),
    MaChuyenKhoa VARCHAR(10) NOT NULL,
    GiaTien DECIMAL(15, 2) NOT NULL,
    ThoiGianTrungBinh INT,
    TrangThai NVARCHAR(20) DEFAULT N'Hoạt động',
    FOREIGN KEY (MaChuyenKhoa) REFERENCES ChuyenKhoa(MaChuyenKhoa) ON UPDATE CASCADE,
    CONSTRAINT CHK_DichVuKham_GiaTien CHECK (GiaTien > 0),
    CONSTRAINT CHK_DichVuKham_ThoiGianTrungBinh CHECK (ThoiGianTrungBinh > 0),
    CONSTRAINT CHK_DichVuKham_TrangThai CHECK (TrangThai IN (N'Hoạt động', N'Ngừng cung cấp'))
)
GO

-- 5. Bảng Bác Sĩ
CREATE TABLE BacSi (
    MaBacSi VARCHAR(10) PRIMARY KEY,
	MaNguoiDung VARCHAR(10) UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    DienThoai VARCHAR(20),
    DiaChi NVARCHAR(200),
    TrinhDo NVARCHAR(100),
    MaChuyenKhoa VARCHAR(10),
    Email VARCHAR(100),
	FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaChuyenKhoa) REFERENCES ChuyenKhoa(MaChuyenKhoa) ON UPDATE CASCADE,
    -- Thêm ràng buộc giới tính
    CONSTRAINT CHK_BacSi_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    -- Thêm ràng buộc ngày sinh hợp lệ (tuổi phải từ 22 đến 70)
    CONSTRAINT CHK_BacSi_NgaySinh CHECK (NgaySinh <= DATEADD(YEAR, -22, GETDATE()) AND NgaySinh >= DATEADD(YEAR, -70, GETDATE())),
    -- Thêm ràng buộc số điện thoại
    CONSTRAINT CHK_BacSi_DienThoai CHECK (LEN(DienThoai) = 10 AND DienThoai NOT LIKE '%[^0-9]%'),
    -- Thêm ràng buộc trình độ
    CONSTRAINT CHK_BacSi_TrinhDo CHECK (TrinhDo IN (N'Bác sĩ', N'Thạc sĩ', N'Tiến sĩ', N'Phó giáo sư', N'Giáo sư'))
);

-- 6. Bảng Lịch Làm Việc
CREATE TABLE LichLamViec (
    MaBacSi VARCHAR(10),
    NgayTrongTuan NVARCHAR(20),
    CaLamViec NVARCHAR(10),
	MaPhongKham VARCHAR(10),
    PRIMARY KEY (MaBacSi, NgayTrongTuan, CaLamViec),
	FOREIGN KEY (MaPhongKham) REFERENCES PhongKham(MaPhongKham),
    FOREIGN KEY (MaBacSi) REFERENCES BacSi(MaBacSi) ON DELETE CASCADE,
    -- Thêm ràng buộc ngày trong tuần
    CONSTRAINT CHK_LichLamViec_NgayTrongTuan CHECK (
		NgayTrongTuan IN (N'Thứ 2', N'Thứ 3', N'Thứ 4', N'Thứ 5', N'Thứ 6', N'Thứ 7', N'Chủ nhật')
	),
    -- Thêm ràng buộc ca làm việc
    CONSTRAINT CHK_LichLamViec_CaLamViec CHECK (CaLamViec IN (N'Sáng', N'Chiều', N'Tối'))
);

-- 7. Bảng Bệnh Nhân
CREATE TABLE BenhNhan (
    MaBenhNhan VARCHAR(10) PRIMARY KEY,
	MaNguoiDung VARCHAR(10) UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10),
    NgaySinh DATE,
    DienThoai VARCHAR(20),
    DiaChi NVARCHAR(200),
	FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    -- Thêm ràng buộc giới tính
    CONSTRAINT CHK_BenhNhan_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ')),
    -- Thêm ràng buộc ngày sinh hợp lệ
    CONSTRAINT CHK_BenhNhan_NgaySinh CHECK (NgaySinh <= CAST(GETDATE() AS DATE)),
    -- Thêm ràng buộc số điện thoại
    CONSTRAINT CHK_BenhNhan_DienThoai CHECK (DienThoai LIKE '[0-9]%' AND LEN(DienThoai) = 10)
);

-- 8. Bảng Đăng Ký Lịch Khám
CREATE TABLE DangKyLichKham (
    MaDangKy VARCHAR(10) PRIMARY KEY,
    MaBacSi VARCHAR(10) NOT NULL,
    MaBenhNhan VARCHAR(10) NOT NULL,
    MaPhongKham VARCHAR(10) NOT NULL,
    NgayKham DATE NOT NULL,
    CaKham NVARCHAR(10) NOT NULL,
    GioKham TIME(0),
    ThoiLuongKham INT,
    TrangThai NVARCHAR(20) DEFAULT N'Chờ khám',
    FOREIGN KEY (MaBacSi) REFERENCES BacSi(MaBacSi),
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
	FOREIGN KEY (MaPhongKham) REFERENCES PhongKham(MaPhongKham),
    -- Thêm ràng buộc ca khám
    CONSTRAINT CHK_DangKyLichKham_CaKham CHECK (CaKham IN (N'Sáng', N'Chiều', N'Tối')),
    -- Thêm ràng buộc khung giờ và thời lượng khám dự kiến
    CONSTRAINT CHK_DangKyLichKham_GioKham_ThoiLuong CHECK (
        (GioKham IS NULL AND ThoiLuongKham IS NULL)
        OR (GioKham IS NOT NULL AND ThoiLuongKham IS NOT NULL)
    ),
    CONSTRAINT CHK_DangKyLichKham_ThoiLuongKham CHECK (ThoiLuongKham IS NULL OR ThoiLuongKham BETWEEN 15 AND 180),
    CONSTRAINT CHK_DangKyLichKham_GioKham_TrongCa CHECK (
        GioKham IS NULL OR ThoiLuongKham IS NULL OR
        (
            (CaKham = N'Sáng' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 450 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 690)
            OR (CaKham = N'Chiều' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 810 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 1020)
            OR (CaKham = N'Tối' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 1080 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 1230)
        )
    ),
    -- Thêm ràng buộc ngày khám phải ở tương lai
    CONSTRAINT CHK_DangKyLichKham_NgayKham CHECK (NgayKham >= CAST(GETDATE() AS DATE)),
    -- Thêm ràng buộc trạng thái
    CONSTRAINT CHK_DangKyLichKham_TrangThai CHECK (TrangThai IN (N'Chờ khám', N'Đang khám', N'Đã khám', N'Hủy'))
);


-- 9. Bảng Phiếu Khám
CREATE TABLE PhieuKham (
    MaPhieuKham VARCHAR(10) PRIMARY KEY,
    MaDangKy VARCHAR(10) NOT NULL,
    TrieuChung NVARCHAR(1000),
    ChanDoan NVARCHAR(1000),
    HuongDieuTri NVARCHAR(1000),
    TrangThai NVARCHAR(20) DEFAULT N'Đang xử lý',
    FOREIGN KEY (MaDangKy) REFERENCES DangKyLichKham(MaDangKy),
    -- Thêm ràng buộc trạng thái
    CONSTRAINT CHK_PhieuKham_TrangThai CHECK (TrangThai IN (N'Đang xử lý', N'Hoàn thành', N'Hủy'))
);

-- 10. Bảng Chi Tiết Dịch Vụ Khám
CREATE TABLE ChiTietDichVuKham (
    MaPhieuKham VARCHAR(10) NOT NULL,
    MaDichVu VARCHAR(10) NOT NULL,
    SoLuong INT NOT NULL DEFAULT 1,
    DonGia DECIMAL(15, 2) NOT NULL,
    ThanhTien AS (SoLuong * DonGia) PERSISTED,
    PRIMARY KEY (MaPhieuKham, MaDichVu),
    FOREIGN KEY (MaPhieuKham) REFERENCES PhieuKham(MaPhieuKham) ON DELETE CASCADE,
    FOREIGN KEY (MaDichVu) REFERENCES DichVuKham(MaDichVu),
    CONSTRAINT CHK_ChiTietDichVuKham_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CHK_ChiTietDichVuKham_DonGia CHECK (DonGia > 0)
)
GO

-- 11. Bảng Thuốc
CREATE TABLE Thuoc (
    MaThuoc VARCHAR(10) PRIMARY KEY,
    TenThuoc NVARCHAR(100) NOT NULL,
    DonViTinh NVARCHAR(20) NOT NULL,
    SoLuongTon INT DEFAULT 0,
    GiaBan DECIMAL(15, 2) NOT NULL,
    CONSTRAINT CHK_Thuoc_SoLuongTon CHECK (SoLuongTon >= 0),
    CONSTRAINT CHK_Thuoc_GiaBan CHECK (GiaBan > 0)
);

-- 12. Bảng Đơn Thuốc
CREATE TABLE DonThuoc (
    MaDonThuoc VARCHAR(10) PRIMARY KEY,
    MaPhieuKham VARCHAR(10) NOT NULL,
    NgayLap DATE DEFAULT GETDATE(),
    GhiChu NVARCHAR(1000),
    TrangThai NVARCHAR(20) DEFAULT N'Chờ cấp thuốc',
    FOREIGN KEY (MaPhieuKham) REFERENCES PhieuKham(MaPhieuKham),
    CONSTRAINT CHK_DonThuoc_TrangThai CHECK (TrangThai IN (N'Chờ cấp thuốc', N'Đã cấp thuốc', N'Hủy'))
);

-- 13. Bảng Chi Tiết Đơn Thuốc
CREATE TABLE ChiTietDonThuoc (
    MaDonThuoc VARCHAR(10) NOT NULL,
    MaThuoc VARCHAR(10) NOT NULL,
    SoLuong INT NOT NULL,
    LieuLuong NVARCHAR(50) NOT NULL,
    SoNgayDung INT NOT NULL,
    CachDung NVARCHAR(1000) NOT NULL,
    DonGia DECIMAL(15, 2) NOT NULL,
	ThanhTien AS (SoLuong * DonGia) PERSISTED,
    PRIMARY KEY (MaDonThuoc, MaThuoc),
    FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc) ON DELETE CASCADE,
    FOREIGN KEY (MaThuoc) REFERENCES Thuoc(MaThuoc),
    CONSTRAINT CHK_ChiTietDonThuoc_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CHK_ChiTietDonThuoc_SoNgayDung CHECK (SoNgayDung > 0),
    CONSTRAINT CHK_ChiTietDonThuoc_DonGia CHECK (DonGia > 0)
);

-- 14. Bảng Hóa Đơn
CREATE TABLE HoaDon (
    MaHoaDon VARCHAR(10) PRIMARY KEY,
    MaBenhNhan VARCHAR(10) NOT NULL,
    NgayLap DATE DEFAULT GETDATE(),
    TongTien DECIMAL(15, 2) DEFAULT 0,
    HinhThucThanhToan NVARCHAR(50),
    TrangThai NVARCHAR(20) DEFAULT N'Chưa thanh toán',
    FOREIGN KEY (MaBenhNhan) REFERENCES BenhNhan(MaBenhNhan),
    CONSTRAINT CHK_HoaDon_HinhThucThanhToan CHECK (
		HinhThucThanhToan IN (N'Tiền mặt', N'Thẻ', N'Chuyển khoản', N'Bảo hiểm')
	),
    CONSTRAINT CHK_HoaDon_TrangThai CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán', N'Đã hủy')),
    CONSTRAINT CHK_HoaDon_TongTien CHECK (TongTien >= 0)
);

-- 15. Bảng Chi Tiết Hóa Đơn
CREATE TABLE ChiTietHoaDon (
	MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
    MaHoaDon VARCHAR(10) NOT NULL,
    MaPhieuKham VARCHAR(10) NOT NULL,
    MaDonThuoc VARCHAR(10),
    TienKham DECIMAL(15, 2) DEFAULT 0,
    TienThuoc DECIMAL(15, 2) DEFAULT 0,
    GhiChu NVARCHAR(1000),
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon) ON DELETE CASCADE,
    FOREIGN KEY (MaPhieuKham) REFERENCES PhieuKham(MaPhieuKham),
    FOREIGN KEY (MaDonThuoc) REFERENCES DonThuoc(MaDonThuoc),
    CONSTRAINT CHK_ChiTietHoaDon_TienKham CHECK (TienKham >= 0),
    CONSTRAINT CHK_ChiTietHoaDon_TienThuoc CHECK (TienThuoc >= 0)
);

