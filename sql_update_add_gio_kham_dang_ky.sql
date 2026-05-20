USE HeThongDatLichVaKhamBenh;
GO

IF COL_LENGTH('dbo.DangKyLichKham', 'GioKham') IS NULL
BEGIN
    ALTER TABLE dbo.DangKyLichKham
    ADD GioKham TIME(0) NULL;
END
GO

IF COL_LENGTH('dbo.DangKyLichKham', 'ThoiLuongKham') IS NULL
BEGIN
    ALTER TABLE dbo.DangKyLichKham
    ADD ThoiLuongKham INT NULL;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_DangKyLichKham_GioKham_ThoiLuong'
      AND parent_object_id = OBJECT_ID('dbo.DangKyLichKham')
)
BEGIN
    ALTER TABLE dbo.DangKyLichKham
    ADD CONSTRAINT CHK_DangKyLichKham_GioKham_ThoiLuong CHECK (
        (GioKham IS NULL AND ThoiLuongKham IS NULL)
        OR (GioKham IS NOT NULL AND ThoiLuongKham IS NOT NULL)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_DangKyLichKham_ThoiLuongKham'
      AND parent_object_id = OBJECT_ID('dbo.DangKyLichKham')
)
BEGIN
    ALTER TABLE dbo.DangKyLichKham
    ADD CONSTRAINT CHK_DangKyLichKham_ThoiLuongKham CHECK (
        ThoiLuongKham IS NULL OR ThoiLuongKham BETWEEN 15 AND 180
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CHK_DangKyLichKham_GioKham_TrongCa'
      AND parent_object_id = OBJECT_ID('dbo.DangKyLichKham')
)
BEGIN
    ALTER TABLE dbo.DangKyLichKham
    ADD CONSTRAINT CHK_DangKyLichKham_GioKham_TrongCa CHECK (
        GioKham IS NULL OR ThoiLuongKham IS NULL OR
        (
            (CaKham = N'Sáng' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 450 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 690)
            OR (CaKham = N'Chiều' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 810 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 1020)
            OR (CaKham = N'Tối' AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) >= 1080 AND DATEDIFF(MINUTE, CAST('00:00:00' AS TIME), GioKham) + ThoiLuongKham <= 1230)
        )
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DangKyLichKham_BacSi_Ngay_Ca_Gio'
      AND object_id = OBJECT_ID('dbo.DangKyLichKham')
)
BEGIN
    CREATE INDEX IX_DangKyLichKham_BacSi_Ngay_Ca_Gio
    ON dbo.DangKyLichKham (MaBacSi, NgayKham, CaKham, GioKham)
    INCLUDE (ThoiLuongKham, TrangThai);
END
GO
