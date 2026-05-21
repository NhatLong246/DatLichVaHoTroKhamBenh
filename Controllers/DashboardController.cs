using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class DashboardController : Controller
{
    private const string RolePatient = "Bệnh nhân";
    private const string RoleDoctor = "Bác sĩ";
    private const string RoleAdmin = "Quản trị";
    private const string TrangThaiChoKham = "Chờ khám";
    private const string TrangThaiDangKham = "Đang khám";
    private const string TrangThaiDaKham = "Đã khám";
    private const string TrangThaiHuy = "Hủy";

    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> BenhNhan()
    {
        var redirect = RequireRole(RolePatient);
        if (redirect != null)
        {
            return redirect;
        }

        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var benhNhan = await _context.BenhNhans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        ViewBag.HoTen = benhNhan?.HoTen ?? HttpContext.Session.GetString("TenDangNhap") ?? RolePatient;
        return View();
    }

    public async Task<IActionResult> BacSi()
    {
        var redirect = RequireRole(RoleDoctor);
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildDoctorDashboardModelAsync();
        return View(model);
    }

    public IActionResult Admin()
    {
        var redirect = RequireRole(RoleAdmin);
        if (redirect != null)
        {
            return redirect;
        }

        ViewBag.Today = DateTime.Now.ToString("dd/MM/yyyy");
        return View();
    }

    private async Task<BacSiDashboardViewModel> BuildDoctorDashboardModelAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var bacSi = await _context.BacSis
            .AsNoTracking()
            .Include(x => x.LichLamViecs)
                .ThenInclude(x => x.MaPhongKhamNavigation)
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        if (bacSi == null)
        {
            return new BacSiDashboardViewModel
            {
                HoTenBacSi = HttpContext.Session.GetString("TenDangNhap") ?? RoleDoctor,
                NgayHienTai = today,
                CaLamViec = "Chưa cập nhật",
                PhongKham = "Chưa cập nhật",
                GhiChuHangCho = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại."
            };
        }

        var appointments = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBenhNhanNavigation)
            .Include(x => x.MaPhongKhamNavigation)
            .Where(x => x.MaBacSi == bacSi.MaBacSi && x.NgayKham == today && x.TrangThai != TrangThaiHuy)
            .OrderBy(x => x.GioKham ?? TimeOnly.MinValue)
            .ThenBy(x => x.MaDangKy)
            .ToListAsync();

        var currentShift = appointments.FirstOrDefault(x => x.TrangThai == TrangThaiDangKham)?.CaKham
            ?? appointments.FirstOrDefault(x => x.TrangThai == TrangThaiChoKham)?.CaKham
            ?? GetCurrentShift();
        var todayName = GetVietnameseDayOfWeek(today);
        var workSchedule = bacSi.LichLamViecs.FirstOrDefault(x =>
            x.NgayTrongTuan == todayName &&
            x.CaLamViec == currentShift);

        return new BacSiDashboardViewModel
        {
            HoTenBacSi = bacSi.HoTen,
            NgayHienTai = today,
            DangChoKham = appointments.Count(x => x.TrangThai == TrangThaiChoKham),
            DaTiepNhan = appointments.Count(x => x.TrangThai == TrangThaiDangKham || x.TrangThai == TrangThaiDaKham),
            CaLamViec = workSchedule?.CaLamViec ?? currentShift,
            PhongKham = workSchedule?.MaPhongKhamNavigation?.TenPhongKham
                ?? appointments.FirstOrDefault()?.MaPhongKhamNavigation.TenPhongKham
                ?? "Chưa cập nhật",
            GhiChuHangCho = appointments.Count == 0
                ? "Hôm nay chưa có bệnh nhân trong lịch khám."
                : $"{appointments.Count(x => x.TrangThai == TrangThaiChoKham)} bệnh nhân đang chờ",
            BenhNhanHomNay = appointments.Select(x => new BacSiDashboardPatientViewModel
            {
                MaDangKy = x.MaDangKy,
                TenBenhNhan = x.MaBenhNhanNavigation.HoTen,
                DienThoai = x.MaBenhNhanNavigation.DienThoai ?? "Chưa cập nhật",
                CaKham = x.CaKham,
                GioKhamText = x.GioKham?.ToString("HH:mm") ?? "Chưa hẹn giờ",
                TenPhongKham = x.MaPhongKhamNavigation.TenPhongKham,
                TrangThai = x.TrangThai ?? TrangThaiChoKham
            }).ToList()
        };
    }

    private IActionResult? RequireRole(string role)
    {
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(currentRole))
        {
            return RedirectToAction("Login", "Account");
        }

        if (currentRole != role)
        {
            return currentRole switch
            {
                RolePatient => RedirectToAction(nameof(BenhNhan)),
                RoleDoctor => RedirectToAction(nameof(BacSi)),
                RoleAdmin => RedirectToAction(nameof(Admin)),
                _ => RedirectToAction("Login", "Account")
            };
        }

        return null;
    }

    private static string GetCurrentShift()
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        if (now >= new TimeOnly(7, 30) && now <= new TimeOnly(11, 30))
        {
            return "Sáng";
        }

        if (now >= new TimeOnly(13, 30) && now <= new TimeOnly(17, 0))
        {
            return "Chiều";
        }

        if (now >= new TimeOnly(18, 0) && now <= new TimeOnly(20, 30))
        {
            return "Tối";
        }

        return "Chưa vào ca";
    }

    private static string GetVietnameseDayOfWeek(DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ 2",
            DayOfWeek.Tuesday => "Thứ 3",
            DayOfWeek.Wednesday => "Thứ 4",
            DayOfWeek.Thursday => "Thứ 5",
            DayOfWeek.Friday => "Thứ 6",
            DayOfWeek.Saturday => "Thứ 7",
            _ => "Chủ nhật"
        };
    }
}
