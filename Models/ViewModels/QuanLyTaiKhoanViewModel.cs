namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class QuanLyTaiKhoanViewModel
{
    public string Search { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<ThongTinTaiKhoanViewModel> DanhSachTaiKhoan { get; set; } = new();
}

public class ThongTinTaiKhoanViewModel
{
    public string MaNguoiDung { get; set; } = null!;
    public string TenDangNhap { get; set; } = null!;
    public string HoTen { get; set; } = null!;
    public string VaiTro { get; set; } = null!;
    public bool TrangThai { get; set; }
    
    // Thuộc tính mở rộng để hiện UI
    public string ChuyenKhoaHoacGioiTinh { get; set; } = string.Empty;
}
