using System.ComponentModel.DataAnnotations;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class DatLichKhamViewModel
{
    [StringLength(1000, ErrorMessage = "Triệu chứng chỉ được nhập tối đa 1000 ký tự.")]
    public string? TrieuChung { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    public string? MaChuyenKhoa { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn bác sĩ.")]
    public string? MaBacSi { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày khám.")]
    [DataType(DataType.Date)]
    public DateTime? NgayKham { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ca khám.")]
    public string? CaKham { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khung giờ khám.")]
    public string? GioKham { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời lượng khám dự kiến.")]
    [Range(15, 180, ErrorMessage = "Thời lượng khám phải từ 15 đến 180 phút.")]
    public int? ThoiLuongKham { get; set; }

    public string? HoTenBenhNhan { get; set; }

    public List<ChuyenKhoaOptionViewModel> ChuyenKhoas { get; set; } = new();

    public List<BacSiOptionViewModel> BacSis { get; set; } = new();

    public List<LichLamViecOptionViewModel> LichLamViecs { get; set; } = new();

    public DateTime MinNgayKham { get; set; } = DateTime.Today;

    public string? SuccessMessage { get; set; }
}

public class ChuyenKhoaOptionViewModel
{
    public string MaChuyenKhoa { get; set; } = string.Empty;

    public string TenChuyenKhoa { get; set; } = string.Empty;
}

public class BacSiOptionViewModel
{
    public string MaBacSi { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string? TrinhDo { get; set; }

    public string? MaChuyenKhoa { get; set; }

    public string TenChuyenKhoa { get; set; } = string.Empty;
}

public class LichLamViecOptionViewModel
{
    public string MaBacSi { get; set; } = string.Empty;

    public string NgayTrongTuan { get; set; } = string.Empty;

    public string CaLamViec { get; set; } = string.Empty;

    public string MaPhongKham { get; set; } = string.Empty;

    public string TenPhongKham { get; set; } = string.Empty;

    public string ViTri { get; set; } = string.Empty;
}
