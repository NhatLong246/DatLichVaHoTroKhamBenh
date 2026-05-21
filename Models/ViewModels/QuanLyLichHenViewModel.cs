namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class QuanLyLichHenViewModel
{
    public string HoTenBenhNhan { get; set; } = string.Empty;

    public int TongLichHen { get; set; }

    public int LichChoKham { get; set; }

    public int LichDaKham { get; set; }

    public int LichDaHuy { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public List<LichHenItemViewModel> LichHen { get; set; } = new();
}

public class LichHenItemViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string TenBacSi { get; set; } = string.Empty;

    public string TrinhDoBacSi { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string ViTriPhongKham { get; set; } = string.Empty;

    public DateOnly NgayKham { get; set; }

    public string CaKham { get; set; } = string.Empty;

    public TimeOnly? GioKham { get; set; }

    public int? ThoiLuongKham { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string? TrieuChung { get; set; }

    public string? ChanDoan { get; set; }

    public bool CoPhieuKham { get; set; }

    public bool CoTheHuy { get; set; }
}
