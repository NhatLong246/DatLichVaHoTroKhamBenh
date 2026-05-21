namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class HoaDonViewModel
{
    public string HoTenBenhNhan { get; set; } = string.Empty;

    public int TongHoaDon { get; set; }

    public int ChuaThanhToan { get; set; }

    public int DaThanhToan { get; set; }

    public decimal TongChiPhi { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public List<HoaDonItemViewModel> HoaDons { get; set; } = new();
}

public class HoaDonItemViewModel
{
    public string MaHoaDon { get; set; } = string.Empty;

    public DateOnly? NgayLap { get; set; }

    public decimal TongTien { get; set; }

    public string HinhThucThanhToan { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public List<ChiTietHoaDonViewModel> ChiTietHoaDons { get; set; } = new();
}

public class ChiTietHoaDonViewModel
{
    public int MaChiTiet { get; set; }

    public string MaPhieuKham { get; set; } = string.Empty;

    public string MaDonThuoc { get; set; } = string.Empty;

    public string MaDangKy { get; set; } = string.Empty;

    public DateOnly? NgayKham { get; set; }

    public string CaKham { get; set; } = string.Empty;

    public string TenBacSi { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public decimal TienKham { get; set; }

    public decimal TienThuoc { get; set; }

    public string GhiChu { get; set; } = string.Empty;

    public decimal ThanhTien => TienKham + TienThuoc;
}
