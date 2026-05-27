USE HeThongDatLichVaKhamBenh
GO

-- Kiểm tra xem cột Email đã tồn tại trong bảng NguoiDung chưa, nếu chưa thì thêm vào
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE Name = N'Email' 
    AND Object_ID = Object_ID(N'NguoiDung')
)
BEGIN
    ALTER TABLE NguoiDung
    ADD Email VARCHAR(100) NULL;
    
    PRINT 'Da them cot Email vao bang NguoiDung.'
END
ELSE
BEGIN
    PRINT 'Cot Email da ton tai trong bang NguoiDung.'
END
GO

-- Kiểm tra xem cột Email đã tồn tại trong bảng BacSi chưa, nếu chưa thì thêm vào
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE Name = N'Email' 
    AND Object_ID = Object_ID(N'BacSi')
)
BEGIN
    ALTER TABLE BacSi
    ADD Email VARCHAR(100) NULL;
    
    PRINT 'Da them cot Email vao bang BacSi.'
END
ELSE
BEGIN
    PRINT 'Cot Email da ton tai trong bang BacSi.'
END
GO
