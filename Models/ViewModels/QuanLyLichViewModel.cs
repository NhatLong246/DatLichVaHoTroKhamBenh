using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class QuanLyLichIndexViewModel
{
    public string Search { get; set; } = string.Empty;
    public string ChuyenKhoaId { get; set; } = string.Empty;
    public List<SelectListItem> DanhSachChuyenKhoa { get; set; } = new();
    public List<BacSiLichItemViewModel> DanhSachBacSi { get; set; } = new();
}

public class BacSiLichItemViewModel
{
    public string MaBacSi { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string ChuyenKhoa { get; set; } = null!;
    public int SoCaLamViec { get; set; }
    public bool TrangThai { get; set; }
}

public class ThietLapLichViewModel
{
    public string MaBacSi { get; set; } = null!;
    public string HoTenBacSi { get; set; } = null!;
    public string TenChuyenKhoa { get; set; } = null!;
    
    // MaChuyenKhoa để lấy đúng danh sách phòng khám thuộc chuyên khoa đó
    public string MaChuyenKhoa { get; set; } = null!;
    
    public List<SelectListItem> DanhSachPhongKham { get; set; } = new();
    
    // Grid 7x3 (Thứ 2 - Chủ nhật, Sáng - Chiều - Tối)
    public List<ScheduleCellViewModel> OTrongLich { get; set; } = new();
}

public class ScheduleCellViewModel
{
    public string NgayTrongTuan { get; set; } = null!; // Thứ 2, Thứ 3...
    public string CaLamViec { get; set; } = null!; // Sáng, Chiều, Tối
    
    // Giá trị đã chọn, rỗng nếu không phân công
    public string? MaPhongKham { get; set; } 
}
