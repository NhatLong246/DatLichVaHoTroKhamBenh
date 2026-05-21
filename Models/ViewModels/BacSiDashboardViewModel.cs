namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class BacSiDashboardViewModel
{
    public string HoTenBacSi { get; set; } = string.Empty;

    public DateOnly NgayHienTai { get; set; }

    public int DangChoKham { get; set; }

    public int DaTiepNhan { get; set; }

    public string CaLamViec { get; set; } = string.Empty;

    public string PhongKham { get; set; } = string.Empty;

    public string GhiChuHangCho { get; set; } = string.Empty;

    public List<BacSiDashboardPatientViewModel> BenhNhanHomNay { get; set; } = new();
}

public class BacSiDashboardPatientViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string TenBenhNhan { get; set; } = string.Empty;

    public string DienThoai { get; set; } = string.Empty;

    public string CaKham { get; set; } = string.Empty;

    public string GioKhamText { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;
}
