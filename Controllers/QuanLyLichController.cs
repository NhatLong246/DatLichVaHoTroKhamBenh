using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class QuanLyLichController : Controller
{
    private readonly ApplicationDbContext _context;
    
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

    public QuanLyLichController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string search, string chuyenKhoaId)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị")
        {
            return RedirectToAction("Login", "Account");
        }

        var query = _context.BacSis
            .Include(b => b.MaChuyenKhoaNavigation)
            .Include(b => b.MaNguoiDungNavigation)
            .Include(b => b.LichLamViecs)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(chuyenKhoaId))
        {
            query = query.Where(b => b.MaChuyenKhoa == chuyenKhoaId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(b => 
                b.HoTen.ToLower().Contains(search) || 
                b.MaBacSi.ToLower().Contains(search));
        }

        var listBacSi = await query.OrderBy(b => b.MaBacSi).ToListAsync();

        var model = new QuanLyLichIndexViewModel
        {
            Search = search ?? "",
            ChuyenKhoaId = chuyenKhoaId ?? "",
            DanhSachChuyenKhoa = await _context.ChuyenKhoas
                .Select(c => new SelectListItem { Value = c.MaChuyenKhoa, Text = c.TenChuyenKhoa })
                .ToListAsync(),
            DanhSachBacSi = listBacSi.Select(b => new BacSiLichItemViewModel
            {
                MaBacSi = b.MaBacSi,
                HoTen = b.HoTen,
                ChuyenKhoa = b.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật",
                SoCaLamViec = b.LichLamViecs.Count,
                TrangThai = b.MaNguoiDungNavigation?.TrangThai ?? false
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Manage(string id)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        var bacSi = await _context.BacSis
            .Include(b => b.MaChuyenKhoaNavigation)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);

        if (bacSi == null) return NotFound();

        var currentSchedules = await _context.LichLamViecs
            .Where(x => x.MaBacSi == id)
            .ToListAsync();

        // Lấy danh sách phòng khám thuộc chuyên khoa của bác sĩ
        // hoặc phòng khám chung (nếu cần thiết, ở đây lấy theo chuyên khoa)
        var phongKhams = await _context.PhongKhams
            .Where(p => p.TrangThai == "Hoạt động" && p.MaChuyenKhoa == bacSi.MaChuyenKhoa)
            .Select(p => new SelectListItem { Value = p.MaPhongKham, Text = p.TenPhongKham + " - " + p.ViTri })
            .ToListAsync();

        var model = new ThietLapLichViewModel
        {
            MaBacSi = bacSi.MaBacSi,
            HoTenBacSi = bacSi.HoTen,
            TenChuyenKhoa = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Không xác định",
            MaChuyenKhoa = bacSi.MaChuyenKhoa ?? "",
            DanhSachPhongKham = phongKhams,
            OTrongLich = new List<ScheduleCellViewModel>()
        };

        // Khởi tạo grid 7x3
        foreach (var day in OrderedDays)
        {
            foreach (var shift in OrderedShifts)
            {
                var existing = currentSchedules.FirstOrDefault(x => x.NgayTrongTuan == day && x.CaLamViec == shift);
                model.OTrongLich.Add(new ScheduleCellViewModel
                {
                    NgayTrongTuan = day,
                    CaLamViec = shift,
                    MaPhongKham = existing?.MaPhongKham
                });
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(string id, ThietLapLichViewModel model)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        if (id != model.MaBacSi) return BadRequest();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Xóa lịch cũ của bác sĩ
            var oldSchedules = await _context.LichLamViecs.Where(x => x.MaBacSi == id).ToListAsync();
            _context.LichLamViecs.RemoveRange(oldSchedules);

            // 2. Thêm lịch mới
            foreach (var cell in model.OTrongLich)
            {
                if (!string.IsNullOrEmpty(cell.MaPhongKham))
                {
                    _context.LichLamViecs.Add(new LichLamViec
                    {
                        MaBacSi = id,
                        NgayTrongTuan = cell.NgayTrongTuan,
                        CaLamViec = cell.CaLamViec,
                        MaPhongKham = cell.MaPhongKham
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật lịch làm việc thành công cho bác sĩ {model.HoTenBacSi}!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Lỗi khi lưu lịch làm việc: " + ex.Message;
            
            // Re-populate dropdown
            model.DanhSachPhongKham = await _context.PhongKhams
                .Where(p => p.TrangThai == "Hoạt động" && p.MaChuyenKhoa == model.MaChuyenKhoa)
                .Select(p => new SelectListItem { Value = p.MaPhongKham, Text = p.TenPhongKham + " - " + p.ViTri })
                .ToListAsync();
                
            return View(model);
        }
    }
}
