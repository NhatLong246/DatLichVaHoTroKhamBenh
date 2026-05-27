using System.Security.Cryptography;
using System.Text;
using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class QuanLyBacSiController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuanLyBacSiController(ApplicationDbContext context)
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
                b.MaBacSi.ToLower().Contains(search) ||
                (b.Email != null && b.Email.ToLower().Contains(search)) ||
                (b.DienThoai != null && b.DienThoai.Contains(search)));
        }

        var listBacSi = await query.OrderBy(b => b.MaBacSi).ToListAsync();

        var model = new QuanLyBacSiViewModel
        {
            Search = search ?? "",
            ChuyenKhoaId = chuyenKhoaId ?? "",
            DanhSachChuyenKhoa = await _context.ChuyenKhoas
                .Select(c => new SelectListItem { Value = c.MaChuyenKhoa, Text = c.TenChuyenKhoa })
                .ToListAsync(),
            DanhSachBacSi = listBacSi.Select(b => new BacSiItemViewModel
            {
                MaBacSi = b.MaBacSi,
                MaNguoiDung = b.MaNguoiDung,
                HoTen = b.HoTen,
                GioiTinh = b.GioiTinh ?? "",
                DienThoai = b.DienThoai ?? "",
                Email = b.Email ?? "",
                ChuyenKhoa = b.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "",
                TrangThai = b.MaNguoiDungNavigation?.TrangThai ?? false
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        var model = new BacSiCreateEditViewModel();
        await PrepareDropdowns(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BacSiCreateEditViewModel model)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        if (string.IsNullOrEmpty(model.TenDangNhap))
            ModelState.AddModelError("TenDangNhap", "Vui lòng nhập tên đăng nhập");
        if (string.IsNullOrEmpty(model.MatKhau))
            ModelState.AddModelError("MatKhau", "Vui lòng nhập mật khẩu");

        if (await _context.NguoiDungs.AnyAsync(x => x.TenDangNhap == model.TenDangNhap))
        {
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
        }

        if (ModelState.IsValid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate IDs
                var nguoiDungs = await _context.NguoiDungs.Select(x => x.MaNguoiDung).ToListAsync();
                var maxNdNum = nguoiDungs.Select(x => int.TryParse(x.Substring(2), out int n) ? n : 0).DefaultIfEmpty(0).Max();
                string newMaNguoiDung = $"ND{(maxNdNum + 1):D3}";

                var bacSis = await _context.BacSis.Select(x => x.MaBacSi).ToListAsync();
                var maxBsNum = bacSis.Select(x => int.TryParse(x.Substring(2), out int n) ? n : 0).DefaultIfEmpty(0).Max();
                string newMaBacSi = $"BS{(maxBsNum + 1):D3}";

                // Create User
                var newUser = new NguoiDung
                {
                    MaNguoiDung = newMaNguoiDung,
                    TenDangNhap = model.TenDangNhap,
                    MatKhau = HashPassword(model.MatKhau!),
                    VaiTro = "Bác sĩ",
                    TrangThai = true,
                    Email = model.Email
                };
                _context.NguoiDungs.Add(newUser);

                // Create Doctor
                var newBacSi = new BacSi
                {
                    MaBacSi = newMaBacSi,
                    MaNguoiDung = newMaNguoiDung,
                    HoTen = model.HoTen,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = model.NgaySinh.HasValue ? DateOnly.FromDateTime(model.NgaySinh.Value) : null,
                    DienThoai = model.DienThoai,
                    DiaChi = model.DiaChi,
                    TrinhDo = model.TrinhDo,
                    MaChuyenKhoa = model.MaChuyenKhoa,
                    Email = model.Email
                };
                _context.BacSis.Add(newBacSi);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Thêm bác sĩ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        await PrepareDropdowns(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        var bacSi = await _context.BacSis
            .Include(b => b.MaNguoiDungNavigation)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);

        if (bacSi == null) return NotFound();

        var model = new BacSiCreateEditViewModel
        {
            MaBacSi = bacSi.MaBacSi,
            MaNguoiDung = bacSi.MaNguoiDung,
            HoTen = bacSi.HoTen,
            GioiTinh = bacSi.GioiTinh,
            NgaySinh = bacSi.NgaySinh.HasValue ? bacSi.NgaySinh.Value.ToDateTime(TimeOnly.MinValue) : null,
            DienThoai = bacSi.DienThoai ?? "",
            DiaChi = bacSi.DiaChi,
            TrinhDo = bacSi.TrinhDo ?? "",
            MaChuyenKhoa = bacSi.MaChuyenKhoa ?? "",
            Email = bacSi.Email,
            TenDangNhap = bacSi.MaNguoiDungNavigation.TenDangNhap,
            // Don't bind password
        };

        await PrepareDropdowns(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, BacSiCreateEditViewModel model)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        if (id != model.MaBacSi) return BadRequest();

        // Mật khẩu không bắt buộc khi edit
        ModelState.Remove("MatKhau");
        ModelState.Remove("TenDangNhap");

        if (ModelState.IsValid)
        {
            var bacSi = await _context.BacSis
                .Include(b => b.MaNguoiDungNavigation)
                .FirstOrDefaultAsync(b => b.MaBacSi == id);

            if (bacSi == null) return NotFound();

            bacSi.HoTen = model.HoTen;
            bacSi.GioiTinh = model.GioiTinh;
            bacSi.NgaySinh = model.NgaySinh.HasValue ? DateOnly.FromDateTime(model.NgaySinh.Value) : null;
            bacSi.DienThoai = model.DienThoai;
            bacSi.DiaChi = model.DiaChi;
            bacSi.TrinhDo = model.TrinhDo;
            bacSi.MaChuyenKhoa = model.MaChuyenKhoa;
            bacSi.Email = model.Email;

            // Sync email to NguoiDung
            if (bacSi.MaNguoiDungNavigation != null)
            {
                bacSi.MaNguoiDungNavigation.Email = model.Email;

                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    bacSi.MaNguoiDungNavigation.MatKhau = HashPassword(model.MatKhau);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thông tin bác sĩ thành công!";
            return RedirectToAction(nameof(Index));
        }

        await PrepareDropdowns(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị") return Unauthorized();

        var bacSi = await _context.BacSis
            .Include(b => b.MaNguoiDungNavigation)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);

        if (bacSi == null) return NotFound();

        // Check related data (DangKyLichKham, LichLamViec)
        var hasSchedules = await _context.LichLamViecs.AnyAsync(x => x.MaBacSi == id);
        var hasAppointments = await _context.DangKyLichKhams.AnyAsync(x => x.MaBacSi == id);

        if (hasSchedules || hasAppointments)
        {
            // Soft delete
            if (bacSi.MaNguoiDungNavigation != null)
            {
                bacSi.MaNguoiDungNavigation.TrangThai = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bác sĩ đã có dữ liệu lịch làm việc/lịch khám nên hệ thống đã chuyển sang trạng thái: Khóa tài khoản.";
            }
        }
        else
        {
            // Hard delete
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.BacSis.Remove(bacSi);
                if (bacSi.MaNguoiDungNavigation != null)
                {
                    _context.NguoiDungs.Remove(bacSi.MaNguoiDungNavigation);
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "Đã xóa bác sĩ thành công.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Không thể xóa do có lỗi ràng buộc dữ liệu. Vui lòng sử dụng chức năng khóa tài khoản.";
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareDropdowns(BacSiCreateEditViewModel model)
    {
        model.DanhSachChuyenKhoa = await _context.ChuyenKhoas
            .Select(c => new SelectListItem { Value = c.MaChuyenKhoa, Text = c.TenChuyenKhoa })
            .ToListAsync();

        var trinhDos = new List<string> { "Bác sĩ", "Thạc sĩ", "Tiến sĩ", "Phó giáo sư", "Giáo sư" };
        model.DanhSachTrinhDo = trinhDos.Select(t => new SelectListItem { Value = t, Text = t }).ToList();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
