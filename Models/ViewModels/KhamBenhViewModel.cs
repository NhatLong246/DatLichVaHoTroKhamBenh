using System.ComponentModel.DataAnnotations;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class KhamBenhViewModel
{
    public string HoTenBacSi { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;

    public DateOnly NgayKham { get; set; }

    public string? SelectedMaDangKy { get; set; }

    public string? SelectedMaPhieuKham { get; set; }

    public int DangChoKham { get; set; }

    public int DangKham { get; set; }

    public int DaHoanThanh { get; set; }

    public decimal TongTienDichVuTamTinh => DichVus
        .Where(x => x.IsSelected)
        .Sum(x => x.DonGia * Math.Max(1, x.SoLuong));

    [StringLength(1000, ErrorMessage = "Triệu chứng không được vượt quá 1000 ký tự.")]
    public string? TrieuChung { get; set; }

    [StringLength(1000, ErrorMessage = "Chẩn đoán không được vượt quá 1000 ký tự.")]
    public string? ChanDoan { get; set; }

    [StringLength(1000, ErrorMessage = "Hướng điều trị không được vượt quá 1000 ký tự.")]
    public string? HuongDieuTri { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú đơn thuốc không được vượt quá 1000 ký tự.")]
    public string? GhiChuDonThuoc { get; set; }

    public KhamBenhPatientViewModel? BenhNhanDangChon { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public List<KhamBenhQueueItemViewModel> HangCho { get; set; } = new();

    public List<ChiDinhDichVuInputViewModel> DichVus { get; set; } = new();

    public List<KeDonThuocInputViewModel> Thuocs { get; set; } = new();
}

public class KhamBenhQueueItemViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string TenBenhNhan { get; set; } = string.Empty;

    public string DienThoai { get; set; } = string.Empty;

    public string GioKhamText { get; set; } = string.Empty;

    public string CaKham { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;

    public bool IsSelected { get; set; }
}

public class KhamBenhPatientViewModel
{
    public string MaDangKy { get; set; } = string.Empty;

    public string MaBenhNhan { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string DienThoai { get; set; } = string.Empty;

    public string GioiTinh { get; set; } = string.Empty;

    public string NgaySinhText { get; set; } = string.Empty;

    public string DiaChi { get; set; } = string.Empty;

    public string CaKham { get; set; } = string.Empty;

    public string GioKhamText { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;
}

public class ChiDinhDichVuInputViewModel
{
    public string MaDichVu { get; set; } = string.Empty;

    public string TenDichVu { get; set; } = string.Empty;

    public string MoTa { get; set; } = string.Empty;

    public decimal DonGia { get; set; }

    public int? ThoiGianTrungBinh { get; set; }

    public bool IsSelected { get; set; }

    [Range(1, 99, ErrorMessage = "Số lượng dịch vụ phải từ 1 đến 99.")]
    public int SoLuong { get; set; } = 1;
}

public class KeDonThuocInputViewModel
{
    public string MaThuoc { get; set; } = string.Empty;

    public string TenThuoc { get; set; } = string.Empty;

    public string DonViTinh { get; set; } = string.Empty;

    public int SoLuongTon { get; set; }

    public decimal DonGia { get; set; }

    public bool IsSelected { get; set; }

    [Range(1, 999, ErrorMessage = "Số lượng thuốc phải lớn hơn 0.")]
    public int SoLuong { get; set; } = 1;

    [StringLength(50, ErrorMessage = "Liều lượng không được vượt quá 50 ký tự.")]
    public string? LieuLuong { get; set; }

    [Range(1, 365, ErrorMessage = "Số ngày dùng phải lớn hơn 0.")]
    public int SoNgayDung { get; set; } = 1;

    [StringLength(1000, ErrorMessage = "Cách dùng không được vượt quá 1000 ký tự.")]
    public string? CachDung { get; set; }
}
