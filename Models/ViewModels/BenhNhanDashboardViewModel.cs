namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class BenhNhanDashboardViewModel
{
    public string HoTen { get; set; } = string.Empty;
    public int LichHenChoKham { get; set; }
    public int HoSoBenhAnDaLuu { get; set; }
    public int HoaDonDaThanhToan { get; set; }
    public UpcomingAppointmentViewModel? LichHenSapToi { get; set; }
}

public class UpcomingAppointmentViewModel
{
    public string Ngay { get; set; } = string.Empty;
    public string Thang { get; set; } = string.Empty;
    public string CaKham { get; set; } = string.Empty;
    public string TenBacSi { get; set; } = string.Empty;
    public string ChuyenKhoa { get; set; } = string.Empty;
    public string TenPhongKham { get; set; } = string.Empty;
    public string GioKham { get; set; } = string.Empty;
}
