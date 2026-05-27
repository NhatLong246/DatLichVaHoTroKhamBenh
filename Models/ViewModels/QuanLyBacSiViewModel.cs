using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class QuanLyBacSiViewModel
{
    public string Search { get; set; } = "";
    public string ChuyenKhoaId { get; set; } = "";
    public List<SelectListItem> DanhSachChuyenKhoa { get; set; } = new List<SelectListItem>();
    public List<BacSiItemViewModel> DanhSachBacSi { get; set; } = new List<BacSiItemViewModel>();
}

public class BacSiItemViewModel
{
    public string MaBacSi { get; set; } = "";
    public string MaNguoiDung { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string GioiTinh { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public string ChuyenKhoa { get; set; } = "";
    public string Email { get; set; } = "";
    public bool TrangThai { get; set; }
}

public class BacSiCreateEditViewModel
{
    public string? MaBacSi { get; set; }
    public string? MaNguoiDung { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = "";

    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string DienThoai { get; set; } = "";

    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn trình độ")]
    [Display(Name = "Trình độ")]
    public string TrinhDo { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa")]
    [Display(Name = "Chuyên khoa")]
    public string MaChuyenKhoa { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    // Account fields
    [Display(Name = "Tên đăng nhập")]
    public string? TenDangNhap { get; set; }

    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    public string? MatKhau { get; set; }

    public List<SelectListItem>? DanhSachChuyenKhoa { get; set; }
    public List<SelectListItem>? DanhSachTrinhDo { get; set; }
}
