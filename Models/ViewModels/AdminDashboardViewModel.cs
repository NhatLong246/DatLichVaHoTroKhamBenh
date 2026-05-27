namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TongBenhNhan { get; set; }
    public int TongBacSi { get; set; }
    public int LichKhamHomNay { get; set; }
    public decimal TongDoanhThu { get; set; }
    
    public List<AdminLichKhamViewModel> DanhSachLichKhamHomNay { get; set; } = new();
    public List<AdminBacSiLamViecViewModel> DanhSachBacSiLamViec { get; set; } = new();
    public List<AdminHoaDonViewModel> DanhSachHoaDonGanDay { get; set; } = new();
}

public class AdminLichKhamViewModel
{
    public string TenBenhNhan { get; set; } = null!;
    public string TenBacSi { get; set; } = null!;
    public string CaKham { get; set; } = null!;
    public string TrangThai { get; set; } = null!;
}

public class AdminBacSiLamViecViewModel
{
    public string TenBacSi { get; set; } = null!;
    public string ChuyenKhoa { get; set; } = null!;
    public string CaLamViec { get; set; } = null!;
    public string Phong { get; set; } = null!;
}

public class AdminHoaDonViewModel
{
    public string MaHoaDon { get; set; } = null!;
    public string TenBenhNhan { get; set; } = null!;
    public string NgayLap { get; set; } = null!;
    public decimal TongTien { get; set; }
    public string TrangThai { get; set; } = null!;
}
