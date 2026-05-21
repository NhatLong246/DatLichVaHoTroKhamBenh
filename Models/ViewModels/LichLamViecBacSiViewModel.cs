namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class LichLamViecBacSiViewModel
{
    public string HoTenBacSi { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;

    public DateOnly TuanBatDau { get; set; }

    public DateOnly TuanKetThuc { get; set; }

    public int TongCaLamViec { get; set; }

    public int TongBenhNhanTrongTuan { get; set; }

    public string CaNhieuBenhNhanNhat { get; set; } = string.Empty;

    public List<LichLamViecDayViewModel> NgayTrongTuan { get; set; } = new();

    public List<string> CaLamViecs { get; set; } = new();

    public List<LichLamViecCellViewModel> OTrongLich { get; set; } = new();
}

public class LichLamViecDayViewModel
{
    public string TenThu { get; set; } = string.Empty;

    public DateOnly Ngay { get; set; }
}

public class LichLamViecCellViewModel
{
    public string Key { get; set; } = string.Empty;

    public string TenThu { get; set; } = string.Empty;

    public DateOnly Ngay { get; set; }

    public string NgayText { get; set; } = string.Empty;

    public string CaLamViec { get; set; } = string.Empty;

    public bool DaPhanCong { get; set; }

    public string TenPhongKham { get; set; } = string.Empty;

    public string ViTriPhongKham { get; set; } = string.Empty;

    public int SoBenhNhan { get; set; }

    public List<LichLamViecPatientViewModel> BenhNhanDatLich { get; set; } = new();
}

public class LichLamViecPatientViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string TenBenhNhan { get; set; } = string.Empty;

    public string DienThoai { get; set; } = string.Empty;

    public string GioKhamText { get; set; } = string.Empty;

    public int? ThoiLuongKham { get; set; }

    public string TrangThai { get; set; } = string.Empty;
}
