namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class HoSoBenhAnViewModel
{
    public string HoTenBenhNhan { get; set; } = string.Empty;

    public int TongLanKham { get; set; }

    public int TongDonThuoc { get; set; }

    public int TongDichVu { get; set; }

    public decimal TongChiPhiDichVu { get; set; }

    public List<LanKhamViewModel> LanKhams { get; set; } = new();
}

public class LanKhamViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string MaPhieuKham { get; set; } = string.Empty;

    public DateOnly NgayKham { get; set; }

    public string CaKham { get; set; } = string.Empty;

    public TimeOnly? GioKham { get; set; }

    public string TrangThaiLich { get; set; } = string.Empty;

    public string TrangThaiPhieu { get; set; } = string.Empty;

    public string TenBacSi { get; set; } = string.Empty;

    public string TrinhDoBacSi { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string ViTriPhongKham { get; set; } = string.Empty;

    public string TrieuChung { get; set; } = string.Empty;

    public string ChanDoan { get; set; } = string.Empty;

    public string HuongDieuTri { get; set; } = string.Empty;

    public List<DonThuocHoSoViewModel> DonThuocs { get; set; } = new();

    public List<DichVuHoSoViewModel> DichVuDaDung { get; set; } = new();

    public decimal TongTienDichVu => DichVuDaDung.Sum(x => x.ThanhTien);
}

public class DonThuocHoSoViewModel
{
    public string MaDonThuoc { get; set; } = string.Empty;

    public DateOnly? NgayLap { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public string GhiChu { get; set; } = string.Empty;

    public List<ThuocHoSoViewModel> Thuocs { get; set; } = new();
}

public class ThuocHoSoViewModel
{
    public string MaThuoc { get; set; } = string.Empty;

    public string TenThuoc { get; set; } = string.Empty;

    public string DonViTinh { get; set; } = string.Empty;

    public int SoLuong { get; set; }

    public string LieuLuong { get; set; } = string.Empty;

    public int SoNgayDung { get; set; }

    public string CachDung { get; set; } = string.Empty;

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }
}

public class DichVuHoSoViewModel
{
    public string MaDichVu { get; set; } = string.Empty;

    public string TenDichVu { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }
}
