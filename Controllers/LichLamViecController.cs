using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class LichLamViecController : Controller
{
    private const string RoleDoctor = "Bác sĩ";
    private const string TrangThaiHuy = "Hủy";

    private static readonly string[] OrderedDays =
    [
        "Thứ 2",
        "Thứ 3",
        "Thứ 4",
        "Thứ 5",
        "Thứ 6",
        "Thứ 7",
        "Chủ nhật"
    ];

    private static readonly string[] OrderedShifts = ["Sáng", "Chiều", "Tối"];

    private readonly ApplicationDbContext _context;

    public LichLamViecController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var redirect = RequireDoctorRole();
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildModelAsync();
        if (model == null)
        {
            TempData["LichLamViecError"] = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại.";
            return RedirectToAction("BacSi", "Dashboard");
        }

        return View(model);
    }

    private async Task<LichLamViecBacSiViewModel?> BuildModelAsync()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        if (string.IsNullOrWhiteSpace(maNguoiDung))
        {
            return null;
        }

        var bacSi = await _context.BacSis
            .AsNoTracking()
            .Include(x => x.MaChuyenKhoaNavigation)
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        if (bacSi == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var weekStart = GetWeekStart(today);
        var weekEnd = weekStart.AddDays(6);
        var days = OrderedDays
            .Select((day, index) => new LichLamViecDayViewModel
            {
                TenThu = day,
                Ngay = weekStart.AddDays(index)
            })
            .ToList();

        var lichLamViecs = await _context.LichLamViecs
            .AsNoTracking()
            .Include(x => x.MaPhongKhamNavigation)
            .Where(x => x.MaBacSi == bacSi.MaBacSi)
            .ToListAsync();

        var lichHenTrongTuan = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBenhNhanNavigation)
            .Where(x =>
                x.MaBacSi == bacSi.MaBacSi &&
                x.NgayKham >= weekStart &&
                x.NgayKham <= weekEnd &&
                x.TrangThai != TrangThaiHuy)
            .OrderBy(x => x.NgayKham)
            .ThenBy(x => x.GioKham ?? TimeOnly.MinValue)
            .ThenBy(x => x.MaDangKy)
            .ToListAsync();

        var cells = new List<LichLamViecCellViewModel>();
        foreach (var day in days)
        {
            foreach (var shift in OrderedShifts)
            {
                var assignment = lichLamViecs.FirstOrDefault(x =>
                    x.NgayTrongTuan == day.TenThu &&
                    x.CaLamViec == shift);
                var appointments = lichHenTrongTuan
                    .Where(x => x.NgayKham == day.Ngay && x.CaKham == shift)
                    .ToList();

                cells.Add(new LichLamViecCellViewModel
                {
                    Key = $"{day.Ngay:yyyyMMdd}-{shift}",
                    TenThu = day.TenThu,
                    Ngay = day.Ngay,
                    NgayText = day.Ngay.ToString("dd/MM/yyyy"),
                    CaLamViec = shift,
                    DaPhanCong = assignment != null,
                    TenPhongKham = assignment?.MaPhongKhamNavigation?.TenPhongKham ?? "Chưa phân công",
                    ViTriPhongKham = assignment?.MaPhongKhamNavigation?.ViTri ?? "Chưa cập nhật vị trí",
                    SoBenhNhan = appointments.Count,
                    BenhNhanDatLich = appointments.Select(x => new LichLamViecPatientViewModel
                    {
                        MaDangKy = x.MaDangKy,
                        TenBenhNhan = x.MaBenhNhanNavigation.HoTen,
                        DienThoai = x.MaBenhNhanNavigation.DienThoai ?? "Chưa cập nhật",
                        GioKhamText = x.GioKham?.ToString("HH:mm") ?? "Chưa hẹn giờ",
                        ThoiLuongKham = x.ThoiLuongKham,
                        TrangThai = x.TrangThai ?? "Chờ khám"
                    }).ToList()
                });
            }
        }

        var busiestCell = cells
            .Where(x => x.DaPhanCong)
            .OrderByDescending(x => x.SoBenhNhan)
            .FirstOrDefault();

        return new LichLamViecBacSiViewModel
        {
            HoTenBacSi = bacSi.HoTen,
            TenChuyenKhoa = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật chuyên khoa",
            TuanBatDau = weekStart,
            TuanKetThuc = weekEnd,
            TongCaLamViec = cells.Count(x => x.DaPhanCong),
            TongBenhNhanTrongTuan = lichHenTrongTuan.Count,
            CaNhieuBenhNhanNhat = busiestCell == null
                ? "Chưa có lịch"
                : $"{busiestCell.TenThu} - {busiestCell.CaLamViec}",
            NgayTrongTuan = days,
            CaLamViecs = OrderedShifts.ToList(),
            OTrongLich = cells
        };
    }

    private IActionResult? RequireDoctorRole()
    {
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(currentRole))
        {
            return RedirectToAction("Login", "Account");
        }

        return currentRole == RoleDoctor ? null : RedirectToAction("BacSi", "Dashboard");
    }

    private static DateOnly GetWeekStart(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-diff);
    }
}
