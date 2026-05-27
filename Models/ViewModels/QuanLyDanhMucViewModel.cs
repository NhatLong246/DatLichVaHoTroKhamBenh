using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class QuanLyDanhMucViewModel
{
    public string ActiveTab { get; set; } = "chuyen-khoa";
    public List<ChuyenKhoaDanhMucItemViewModel> ChuyenKhoas { get; set; } = new();
    public List<PhongKhamDanhMucItemViewModel> PhongKhams { get; set; } = new();
    public List<DichVuDanhMucItemViewModel> DichVus { get; set; } = new();
    public List<SelectListItem> DanhSachChuyenKhoa { get; set; } = new();
}

public class ChuyenKhoaDanhMucItemViewModel
{
    public string MaChuyenKhoa { get; set; } = string.Empty;
    public string TenChuyenKhoa { get; set; } = string.Empty;
    public int SoPhongKham { get; set; }
    public int SoDichVu { get; set; }
}

public class PhongKhamDanhMucItemViewModel
{
    public string MaPhongKham { get; set; } = string.Empty;
    public string TenPhongKham { get; set; } = string.Empty;
    public string MaChuyenKhoa { get; set; } = string.Empty;
    public string TenChuyenKhoa { get; set; } = string.Empty;
    public string ViTri { get; set; } = string.Empty;
    public int SucChua { get; set; }
    public string TrangThai { get; set; } = "Hoạt động";
}

public class DichVuDanhMucItemViewModel
{
    public string MaDichVu { get; set; } = string.Empty;
    public string TenDichVu { get; set; } = string.Empty;
    public string MaChuyenKhoa { get; set; } = string.Empty;
    public string TenChuyenKhoa { get; set; } = string.Empty;
    public decimal GiaTien { get; set; }
    public int? ThoiGianTrungBinh { get; set; }
    public string MoTa { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Hoạt động";
}

public class TaoChuyenKhoaInputModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên chuyên khoa.")]
    [StringLength(100)]
    public string TenChuyenKhoa { get; set; } = string.Empty;
}

public class CapNhatChuyenKhoaInputModel
{
    [Required]
    [StringLength(10)]
    public string MaChuyenKhoa { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên chuyên khoa.")]
    [StringLength(100)]
    public string TenChuyenKhoa { get; set; } = string.Empty;
}

public class TaoPhongKhamInputModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên phòng khám.")]
    [StringLength(100)]
    public string TenPhongKham { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    [StringLength(10)]
    public string MaChuyenKhoa { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ViTri { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0.")]
    public int SucChua { get; set; } = 1;

    [Required]
    [StringLength(20)]
    public string TrangThai { get; set; } = "Hoạt động";
}

public class CapNhatPhongKhamInputModel : TaoPhongKhamInputModel
{
    [Required]
    [StringLength(10)]
    public string MaPhongKham { get; set; } = string.Empty;
}

public class TaoDichVuInputModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
    [StringLength(100)]
    public string TenDichVu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    [StringLength(10)]
    public string MaChuyenKhoa { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999", ErrorMessage = "Giá dịch vụ phải lớn hơn 0.")]
    public decimal GiaTien { get; set; }

    [Range(1, 10000, ErrorMessage = "Thời gian trung bình phải lớn hơn 0.")]
    public int? ThoiGianTrungBinh { get; set; }

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [Required]
    [StringLength(20)]
    public string TrangThai { get; set; } = "Hoạt động";
}

public class CapNhatDichVuInputModel : TaoDichVuInputModel
{
    [Required]
    [StringLength(10)]
    public string MaDichVu { get; set; } = string.Empty;
}
