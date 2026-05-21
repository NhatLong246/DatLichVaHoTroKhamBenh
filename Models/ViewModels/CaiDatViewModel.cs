using System.ComponentModel.DataAnnotations;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class CaiDatViewModel
{
    public string MaBenhNhan { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string GioiTinh { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số.")]
    public string? DienThoai { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ chỉ được nhập tối đa 200 ký tự.")]
    public string? DiaChi { get; set; }

    public string MaNguoiDung { get; set; } = string.Empty;

    public string TenDangNhap { get; set; } = string.Empty;

    public string VaiTro { get; set; } = string.Empty;

    public string TrangThaiTaiKhoan { get; set; } = string.Empty;

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }
}

public class CaiDatBacSiViewModel
{
    public string MaBacSi { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string GioiTinh { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }

    public string TrinhDo { get; set; } = string.Empty;

    public string ChuyenKhoa { get; set; } = string.Empty;

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số.")]
    public string? DienThoai { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ chỉ được nhập tối đa 200 ký tự.")]
    public string? DiaChi { get; set; }

    public string MaNguoiDung { get; set; } = string.Empty;

    public string TenDangNhap { get; set; } = string.Empty;

    public string VaiTro { get; set; } = string.Empty;

    public string TrangThaiTaiKhoan { get; set; } = string.Empty;

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }
}

public class DoiMatKhauViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    public string MatKhauHienTai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
    public string MatKhauMoi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string XacNhanMatKhauMoi { get; set; } = string.Empty;
}
