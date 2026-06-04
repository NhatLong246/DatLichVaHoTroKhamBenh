-- ==============================================================================
-- Script thêm tính năng EMR (Electronic Medical Record)
-- ==============================================================================

USE HeThongDatLichVaKhamBenh
GO

-- 1. Bảng Chỉ Số Sinh Tồn (Vital Signs)
CREATE TABLE ChiSoSinhTon (
    MaChiSo INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKham VARCHAR(10) NOT NULL,
    HuyetApTamThu INT, -- Systolic BP (mmHg)
    HuyetApTamTruong INT, -- Diastolic BP (mmHg)
    NhipTim INT, -- Heart rate (bpm)
    ChieuCao DECIMAL(5, 2), -- Chiều cao (cm)
    CanNang DECIMAL(5, 2), -- Cân nặng (kg)
    BMI DECIMAL(5, 2), -- Chỉ số khối cơ thể
    DuongHuyet DECIMAL(5, 2), -- Lượng đường trong máu (mmol/L)
    NgayDo DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(1000),
    FOREIGN KEY (MaPhieuKham) REFERENCES PhieuKham(MaPhieuKham) ON DELETE CASCADE,
    -- Thêm các ràng buộc cơ bản
    CONSTRAINT CHK_ChiSoSinhTon_NhipTim CHECK (NhipTim IS NULL OR NhipTim > 0),
    CONSTRAINT CHK_ChiSoSinhTon_ChieuCao CHECK (ChieuCao IS NULL OR ChieuCao > 0),
    CONSTRAINT CHK_ChiSoSinhTon_CanNang CHECK (CanNang IS NULL OR CanNang > 0)
)
GO

-- 2. Bảng Hồ Sơ DICOM (Hình ảnh Y tế)
CREATE TABLE HoSoDicom (
    MaHoSo INT IDENTITY(1,1) PRIMARY KEY,
    MaPhieuKham VARCHAR(10) NOT NULL,
    TenFile NVARCHAR(255) NOT NULL,
    DuongDanFile NVARCHAR(1000) NOT NULL,
    LoaiHinhAnh NVARCHAR(50), -- X-Ray, MRI, CT, Ultrasound...
    NgayTaiLen DATETIME DEFAULT GETDATE(),
    KichThuoc DECIMAL(10, 2), -- Kích thước file (MB)
    FOREIGN KEY (MaPhieuKham) REFERENCES PhieuKham(MaPhieuKham) ON DELETE CASCADE
)
GO
