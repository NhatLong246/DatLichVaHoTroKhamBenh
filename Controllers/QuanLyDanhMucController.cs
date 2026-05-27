using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class QuanLyDanhMucController : Controller
{
    private readonly ApplicationDbContext _context;

    private static readonly HashSet<string> TrangThaiPhongKhamHopLe = new(StringComparer.Ordinal)
    {
        "Hoạt động", "Tạm dừng", "Bảo trì"
    };

    private static readonly HashSet<string> TrangThaiDichVuHopLe = new(StringComparer.Ordinal)
    {
        "Hoạt động", "Ngừng cung cấp"
    };

    public QuanLyDanhMucController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = "chuyen-khoa")
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị")
        {
            return RedirectToAction("Login", "Account");
        }

        var model = await BuildViewModelAsync(tab);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaoChuyenKhoa(TaoChuyenKhoaInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var tenChuyenKhoa = (input.TenChuyenKhoa ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(tenChuyenKhoa))
        {
            TempData["ErrorMessage"] = "Tên chuyên khoa không được để trống.";
            return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
        }

        var daTonTai = await _context.ChuyenKhoas.AnyAsync(x => x.TenChuyenKhoa == tenChuyenKhoa);
        if (daTonTai)
        {
            TempData["ErrorMessage"] = "Tên chuyên khoa đã tồn tại.";
            return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
        }

        var maMoi = await SinhMaMoiAsync(_context.ChuyenKhoas.Select(x => x.MaChuyenKhoa), "CK");
        _context.ChuyenKhoas.Add(new ChuyenKhoa
        {
            MaChuyenKhoa = maMoi,
            TenChuyenKhoa = tenChuyenKhoa
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã thêm chuyên khoa mới.";
        return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatChuyenKhoa(CapNhatChuyenKhoaInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var entity = await _context.ChuyenKhoas.FirstOrDefaultAsync(x => x.MaChuyenKhoa == input.MaChuyenKhoa);
        if (entity == null) return NotFound();

        var tenMoi = (input.TenChuyenKhoa ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(tenMoi))
        {
            TempData["ErrorMessage"] = "Tên chuyên khoa không được để trống.";
            return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
        }

        var trungTen = await _context.ChuyenKhoas.AnyAsync(x => x.MaChuyenKhoa != input.MaChuyenKhoa && x.TenChuyenKhoa == tenMoi);
        if (trungTen)
        {
            TempData["ErrorMessage"] = "Tên chuyên khoa đã tồn tại.";
            return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
        }

        entity.TenChuyenKhoa = tenMoi;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật chuyên khoa.";
        return RedirectToAction(nameof(Index), new { tab = "chuyen-khoa" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaoPhongKham(TaoPhongKhamInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var validationMessage = await ValidatePhongKhamInputAsync(input.MaChuyenKhoa, input.SucChua, input.TrangThai);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            TempData["ErrorMessage"] = validationMessage;
            return RedirectToAction(nameof(Index), new { tab = "phong-kham" });
        }

        var maMoi = await SinhMaMoiAsync(_context.PhongKhams.Select(x => x.MaPhongKham), "PK");
        _context.PhongKhams.Add(new PhongKham
        {
            MaPhongKham = maMoi,
            TenPhongKham = (input.TenPhongKham ?? string.Empty).Trim(),
            MaChuyenKhoa = input.MaChuyenKhoa,
            ViTri = string.IsNullOrWhiteSpace(input.ViTri) ? null : input.ViTri.Trim(),
            SucChua = input.SucChua,
            TrangThai = input.TrangThai,
            GhiChu = null
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã thêm phòng khám mới.";
        return RedirectToAction(nameof(Index), new { tab = "phong-kham" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatPhongKham(CapNhatPhongKhamInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var entity = await _context.PhongKhams.FirstOrDefaultAsync(x => x.MaPhongKham == input.MaPhongKham);
        if (entity == null) return NotFound();

        var validationMessage = await ValidatePhongKhamInputAsync(input.MaChuyenKhoa, input.SucChua, input.TrangThai);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            TempData["ErrorMessage"] = validationMessage;
            return RedirectToAction(nameof(Index), new { tab = "phong-kham" });
        }

        entity.TenPhongKham = (input.TenPhongKham ?? string.Empty).Trim();
        entity.MaChuyenKhoa = input.MaChuyenKhoa;
        entity.ViTri = string.IsNullOrWhiteSpace(input.ViTri) ? null : input.ViTri.Trim();
        entity.SucChua = input.SucChua;
        entity.TrangThai = input.TrangThai;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật phòng khám.";
        return RedirectToAction(nameof(Index), new { tab = "phong-kham" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TaoDichVu(TaoDichVuInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var validationMessage = await ValidateDichVuInputAsync(input.MaChuyenKhoa, input.GiaTien, input.ThoiGianTrungBinh, input.TrangThai);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            TempData["ErrorMessage"] = validationMessage;
            return RedirectToAction(nameof(Index), new { tab = "dich-vu" });
        }

        var maMoi = await SinhMaMoiAsync(_context.DichVuKhams.Select(x => x.MaDichVu), "DV");
        _context.DichVuKhams.Add(new DichVuKham
        {
            MaDichVu = maMoi,
            TenDichVu = (input.TenDichVu ?? string.Empty).Trim(),
            MaChuyenKhoa = input.MaChuyenKhoa,
            GiaTien = input.GiaTien,
            ThoiGianTrungBinh = input.ThoiGianTrungBinh,
            MoTa = string.IsNullOrWhiteSpace(input.MoTa) ? null : input.MoTa.Trim(),
            TrangThai = input.TrangThai
        });

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã thêm dịch vụ mới.";
        return RedirectToAction(nameof(Index), new { tab = "dich-vu" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatDichVu(CapNhatDichVuInputModel input)
    {
        if (!CheckAdmin()) return Unauthorized();

        var entity = await _context.DichVuKhams.FirstOrDefaultAsync(x => x.MaDichVu == input.MaDichVu);
        if (entity == null) return NotFound();

        var validationMessage = await ValidateDichVuInputAsync(input.MaChuyenKhoa, input.GiaTien, input.ThoiGianTrungBinh, input.TrangThai);
        if (!string.IsNullOrEmpty(validationMessage))
        {
            TempData["ErrorMessage"] = validationMessage;
            return RedirectToAction(nameof(Index), new { tab = "dich-vu" });
        }

        entity.TenDichVu = (input.TenDichVu ?? string.Empty).Trim();
        entity.MaChuyenKhoa = input.MaChuyenKhoa;
        entity.GiaTien = input.GiaTien;
        entity.ThoiGianTrungBinh = input.ThoiGianTrungBinh;
        entity.MoTa = string.IsNullOrWhiteSpace(input.MoTa) ? null : input.MoTa.Trim();
        entity.TrangThai = input.TrangThai;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đã cập nhật dịch vụ.";
        return RedirectToAction(nameof(Index), new { tab = "dich-vu" });
    }

    private bool CheckAdmin()
    {
        return HttpContext.Session.GetString("VaiTro") == "Quản trị";
    }

    private async Task<QuanLyDanhMucViewModel> BuildViewModelAsync(string tab)
    {
        var chuyenKhoaStats = await _context.ChuyenKhoas
            .AsNoTracking()
            .OrderBy(x => x.MaChuyenKhoa)
            .Select(x => new ChuyenKhoaDanhMucItemViewModel
            {
                MaChuyenKhoa = x.MaChuyenKhoa,
                TenChuyenKhoa = x.TenChuyenKhoa,
                SoPhongKham = x.PhongKhams.Count,
                SoDichVu = x.DichVuKhams.Count
            })
            .ToListAsync();

        var phongKhamItems = await _context.PhongKhams
            .Include(x => x.MaChuyenKhoaNavigation)
            .AsNoTracking()
            .OrderBy(x => x.MaPhongKham)
            .Select(x => new PhongKhamDanhMucItemViewModel
            {
                MaPhongKham = x.MaPhongKham,
                TenPhongKham = x.TenPhongKham,
                MaChuyenKhoa = x.MaChuyenKhoa,
                TenChuyenKhoa = x.MaChuyenKhoaNavigation.TenChuyenKhoa,
                ViTri = x.ViTri ?? string.Empty,
                SucChua = x.SucChua,
                TrangThai = x.TrangThai ?? "Hoạt động"
            })
            .ToListAsync();

        var dichVuItems = await _context.DichVuKhams
            .Include(x => x.MaChuyenKhoaNavigation)
            .AsNoTracking()
            .OrderBy(x => x.MaDichVu)
            .Select(x => new DichVuDanhMucItemViewModel
            {
                MaDichVu = x.MaDichVu,
                TenDichVu = x.TenDichVu,
                MaChuyenKhoa = x.MaChuyenKhoa,
                TenChuyenKhoa = x.MaChuyenKhoaNavigation.TenChuyenKhoa,
                GiaTien = x.GiaTien,
                ThoiGianTrungBinh = x.ThoiGianTrungBinh,
                MoTa = x.MoTa ?? string.Empty,
                TrangThai = x.TrangThai ?? "Hoạt động"
            })
            .ToListAsync();

        var danhSachChuyenKhoa = await _context.ChuyenKhoas
            .AsNoTracking()
            .OrderBy(x => x.TenChuyenKhoa)
            .Select(x => new SelectListItem
            {
                Value = x.MaChuyenKhoa,
                Text = x.TenChuyenKhoa
            })
            .ToListAsync();

        return new QuanLyDanhMucViewModel
        {
            ActiveTab = tab,
            ChuyenKhoas = chuyenKhoaStats,
            PhongKhams = phongKhamItems,
            DichVus = dichVuItems,
            DanhSachChuyenKhoa = danhSachChuyenKhoa
        };
    }

    private async Task<string?> ValidatePhongKhamInputAsync(string? maChuyenKhoa, int sucChua, string? trangThai)
    {
        if (string.IsNullOrWhiteSpace(maChuyenKhoa) || !await _context.ChuyenKhoas.AnyAsync(x => x.MaChuyenKhoa == maChuyenKhoa))
        {
            return "Chuyên khoa không hợp lệ.";
        }

        if (sucChua <= 0)
        {
            return "Sức chứa phải lớn hơn 0.";
        }

        if (string.IsNullOrWhiteSpace(trangThai) || !TrangThaiPhongKhamHopLe.Contains(trangThai))
        {
            return "Trạng thái phòng khám không hợp lệ.";
        }

        return null;
    }

    private async Task<string?> ValidateDichVuInputAsync(string? maChuyenKhoa, decimal giaTien, int? thoiGianTrungBinh, string? trangThai)
    {
        if (string.IsNullOrWhiteSpace(maChuyenKhoa) || !await _context.ChuyenKhoas.AnyAsync(x => x.MaChuyenKhoa == maChuyenKhoa))
        {
            return "Chuyên khoa không hợp lệ.";
        }

        if (giaTien <= 0)
        {
            return "Giá dịch vụ phải lớn hơn 0.";
        }

        if (thoiGianTrungBinh.HasValue && thoiGianTrungBinh.Value <= 0)
        {
            return "Thời gian trung bình phải lớn hơn 0.";
        }

        if (string.IsNullOrWhiteSpace(trangThai) || !TrangThaiDichVuHopLe.Contains(trangThai))
        {
            return "Trạng thái dịch vụ không hợp lệ.";
        }

        return null;
    }

    private static async Task<string> SinhMaMoiAsync(IQueryable<string> query, string prefix)
    {
        var ids = await query.ToListAsync();
        var max = ids
            .Where(x => !string.IsNullOrWhiteSpace(x) && x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(x =>
            {
                var numberPart = x.Substring(prefix.Length);
                return int.TryParse(numberPart, out var parsed) ? parsed : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}{(max + 1):D3}";
    }
}
