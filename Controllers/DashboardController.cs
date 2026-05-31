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

        if (benhNhan == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        var lichHenChoKhamCount = await _context.DangKyLichKhams
            .CountAsync(x => x.MaBenhNhan == benhNhan.MaBenhNhan && (x.TrangThai == TrangThaiChoKham || x.TrangThai == TrangThaiDangKham));

        var hoSoDaLuuCount = await _context.PhieuKhams
            .CountAsync(x => x.MaDangKyNavigation.MaBenhNhan == benhNhan.MaBenhNhan);

        var hoaDonDaThanhToanCount = await _context.HoaDons
            .CountAsync(x => x.MaBenhNhan == benhNhan.MaBenhNhan && x.TrangThai == "Đã thanh toán");

        var upcomingAppointment = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBacSiNavigation)
                .ThenInclude(b => b.MaChuyenKhoaNavigation)
            .Include(x => x.MaPhongKhamNavigation)
            .Where(x => x.MaBenhNhan == benhNhan.MaBenhNhan && x.TrangThai == TrangThaiChoKham && (x.NgayKham > today || (x.NgayKham == today && x.GioKham >= currentTime)))
            .OrderBy(x => x.NgayKham)
            .ThenBy(x => x.GioKham)
            .FirstOrDefaultAsync();

        UpcomingAppointmentViewModel? upcomingVm = null;
        if (upcomingAppointment != null)
        {
            upcomingVm = new UpcomingAppointmentViewModel
            {
                Ngay = upcomingAppointment.NgayKham.Day.ToString("D2"),
                Thang = $"Tháng {upcomingAppointment.NgayKham.Month}",
                CaKham = upcomingAppointment.CaKham ?? "",
                TenBacSi = upcomingAppointment.MaBacSiNavigation?.HoTen ?? "",
                ChuyenKhoa = upcomingAppointment.MaBacSiNavigation?.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "",
                TenPhongKham = upcomingAppointment.MaPhongKhamNavigation?.TenPhongKham ?? "",
                GioKham = upcomingAppointment.GioKham?.ToString("HH:mm") ?? ""
            };
        }

        var model = new BenhNhanDashboardViewModel
        {
            HoTen = benhNhan.HoTen ?? HttpContext.Session.GetString("TenDangNhap") ?? RolePatient,
            LichHenChoKham = lichHenChoKhamCount,
            HoSoBenhAnDaLuu = hoSoDaLuuCount,
            HoaDonDaThanhToan = hoaDonDaThanhToanCount,
            LichHenSapToi = upcomingVm
        };

        return View(model);
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

    public async Task<IActionResult> Admin()
    {
        var redirect = RequireRole(RoleAdmin);
        if (redirect != null)
        {
            return redirect;
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var todayName = GetVietnameseDayOfWeek(today);

        var tongBenhNhan = await _context.BenhNhans.CountAsync();
        var tongBacSi = await _context.BacSis.CountAsync();
        
        var lichKhamHomNayCount = await _context.DangKyLichKhams
            .Where(x => x.NgayKham == today)
            .CountAsync();

        var tongDoanhThu = await _context.HoaDons
            .Where(x => x.TrangThai == "Đã thanh toán")
            .SumAsync(x => (decimal?)x.TongTien) ?? 0;

        var danhSachLichKham = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBenhNhanNavigation)
            .Include(x => x.MaBacSiNavigation)
            .Where(x => x.NgayKham == today)
            .OrderBy(x => x.GioKham ?? TimeOnly.MinValue)
            .Take(10)
            .Select(x => new AdminLichKhamViewModel
            {
                TenBenhNhan = x.MaBenhNhanNavigation.HoTen,
                TenBacSi = x.MaBacSiNavigation.HoTen,
                CaKham = x.CaKham,
                TrangThai = x.TrangThai ?? "Chờ khám"
            })
            .ToListAsync();

        var danhSachBacSi = await _context.LichLamViecs
            .AsNoTracking()
            .Include(x => x.MaBacSiNavigation)
                .ThenInclude(b => b.MaChuyenKhoaNavigation)
            .Include(x => x.MaPhongKhamNavigation)
            .Where(x => x.NgayTrongTuan == todayName)
            .Take(10)
            .Select(x => new AdminBacSiLamViecViewModel
            {
                TenBacSi = x.MaBacSiNavigation.HoTen,
                ChuyenKhoa = x.MaBacSiNavigation.MaChuyenKhoaNavigation != null ? x.MaBacSiNavigation.MaChuyenKhoaNavigation.TenChuyenKhoa : "N/A",
                CaLamViec = x.CaLamViec,
                Phong = x.MaPhongKhamNavigation != null ? x.MaPhongKhamNavigation.TenPhongKham : "N/A"
            })
            .ToListAsync();

        var danhSachHoaDon = await _context.HoaDons
            .AsNoTracking()
            .Include(x => x.MaBenhNhanNavigation)
            .OrderByDescending(x => x.NgayLap)
            .ThenByDescending(x => x.MaHoaDon)
            .Take(7)
            .Select(x => new AdminHoaDonViewModel
            {
                MaHoaDon = x.MaHoaDon,
                TenBenhNhan = x.MaBenhNhanNavigation.HoTen,
                NgayLap = x.NgayLap.HasValue ? x.NgayLap.Value.ToString("dd/MM/yyyy") : "N/A",
                TongTien = x.TongTien ?? 0,
                TrangThai = x.TrangThai ?? "Chưa thanh toán"
            })
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TongBenhNhan = tongBenhNhan,
            TongBacSi = tongBacSi,
            LichKhamHomNay = lichKhamHomNayCount,
            TongDoanhThu = tongDoanhThu,
            DanhSachLichKhamHomNay = danhSachLichKham,
            DanhSachBacSiLamViec = danhSachBacSi,
            DanhSachHoaDonGanDay = danhSachHoaDon
        };

        ViewBag.Today = DateTime.Now.ToString("dd/MM/yyyy");
        return View(model);
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
