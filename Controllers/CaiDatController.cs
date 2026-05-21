using System.Security.Cryptography;
using System.Text;
using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class CaiDatController : Controller
{
    private const string RolePatient = "Bệnh nhân";
    private const string RoleDoctor = "Bác sĩ";

    private readonly ApplicationDbContext _context;

    public CaiDatController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var redirect = RequireRole(RolePatient);
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildSettingsModelAsync();
        return model == null ? RedirectToAction("BenhNhan", "Dashboard") : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> BacSi()
    {
        var redirect = RequireRole(RoleDoctor);
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildDoctorSettingsModelAsync();
        return model == null ? RedirectToAction("BacSi", "Dashboard") : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTin(CaiDatViewModel model)
    {
        var redirect = RequireRole(RolePatient);
        if (redirect != null)
        {
            return redirect;
        }

        var benhNhan = await GetCurrentPatientQuery().FirstOrDefaultAsync();
        if (benhNhan == null)
        {
            TempData["CaiDatError"] = "Không tìm thấy hồ sơ bệnh nhân cho tài khoản hiện tại.";
            return RedirectToAction("BenhNhan", "Dashboard");
        }

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildSettingsModelAsync();
            if (viewModel == null)
            {
                return RedirectToAction("BenhNhan", "Dashboard");
            }

            viewModel.DienThoai = model.DienThoai;
            viewModel.DiaChi = model.DiaChi;
            return View("Index", viewModel);
        }

        benhNhan.DienThoai = string.IsNullOrWhiteSpace(model.DienThoai) ? null : model.DienThoai.Trim();
        benhNhan.DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim();
        await _context.SaveChangesAsync();

        TempData["CaiDatSuccess"] = "Đã cập nhật thông tin liên hệ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTinBacSi(CaiDatBacSiViewModel model)
    {
        var redirect = RequireRole(RoleDoctor);
        if (redirect != null)
        {
            return redirect;
        }

        var bacSi = await GetCurrentDoctorQuery().FirstOrDefaultAsync();
        if (bacSi == null)
        {
            TempData["CaiDatBacSiError"] = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại.";
            return RedirectToAction("BacSi", "Dashboard");
        }

        if (!ModelState.IsValid)
        {
            var viewModel = await BuildDoctorSettingsModelAsync();
            if (viewModel == null)
            {
                return RedirectToAction("BacSi", "Dashboard");
            }

            viewModel.DienThoai = model.DienThoai;
            viewModel.DiaChi = model.DiaChi;
            return View("BacSi", viewModel);
        }

        bacSi.DienThoai = string.IsNullOrWhiteSpace(model.DienThoai) ? null : model.DienThoai.Trim();
        bacSi.DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim();
        await _context.SaveChangesAsync();

        TempData["CaiDatBacSiSuccess"] = "Đã cập nhật thông tin liên hệ.";
        return RedirectToAction(nameof(BacSi));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
    {
        var redirect = RequireRole(RolePatient);
        if (redirect != null)
        {
            return redirect;
        }

        if (!ModelState.IsValid)
        {
            TempData["CaiDatError"] = "Vui lòng kiểm tra lại thông tin đổi mật khẩu.";
            return RedirectToAction(nameof(Index));
        }

        var result = await ChangePasswordForCurrentUserAsync(model);
        TempData[result.Success ? "CaiDatSuccess" : "CaiDatError"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhauBacSi(DoiMatKhauViewModel model)
    {
        var redirect = RequireRole(RoleDoctor);
        if (redirect != null)
        {
            return redirect;
        }

        if (!ModelState.IsValid)
        {
            TempData["CaiDatBacSiError"] = "Vui lòng kiểm tra lại thông tin đổi mật khẩu.";
            return RedirectToAction(nameof(BacSi));
        }

        var result = await ChangePasswordForCurrentUserAsync(model);
        TempData[result.Success ? "CaiDatBacSiSuccess" : "CaiDatBacSiError"] = result.Message;
        return RedirectToAction(nameof(BacSi));
    }

    private async Task<CaiDatViewModel?> BuildSettingsModelAsync()
    {
        var benhNhan = await GetCurrentPatientQuery()
            .AsNoTracking()
            .Include(x => x.MaNguoiDungNavigation)
            .FirstOrDefaultAsync();

        if (benhNhan == null)
        {
            return null;
        }

        var nguoiDung = benhNhan.MaNguoiDungNavigation;
        return new CaiDatViewModel
        {
            MaBenhNhan = benhNhan.MaBenhNhan,
            HoTen = benhNhan.HoTen,
            GioiTinh = benhNhan.GioiTinh ?? "Chưa cập nhật",
            NgaySinh = benhNhan.NgaySinh,
            DienThoai = benhNhan.DienThoai,
            DiaChi = benhNhan.DiaChi,
            MaNguoiDung = nguoiDung.MaNguoiDung,
            TenDangNhap = nguoiDung.TenDangNhap ?? "Chưa cập nhật",
            VaiTro = nguoiDung.VaiTro ?? "Chưa cập nhật",
            TrangThaiTaiKhoan = nguoiDung.TrangThai == true ? "Đang hoạt động" : "Đã khóa",
            SuccessMessage = TempData["CaiDatSuccess"] as string,
            ErrorMessage = TempData["CaiDatError"] as string
        };
    }

    private async Task<CaiDatBacSiViewModel?> BuildDoctorSettingsModelAsync()
    {
        var bacSi = await GetCurrentDoctorQuery()
            .AsNoTracking()
            .Include(x => x.MaNguoiDungNavigation)
            .Include(x => x.MaChuyenKhoaNavigation)
            .FirstOrDefaultAsync();

        if (bacSi == null)
        {
            return null;
        }

        var nguoiDung = bacSi.MaNguoiDungNavigation;
        return new CaiDatBacSiViewModel
        {
            MaBacSi = bacSi.MaBacSi,
            HoTen = bacSi.HoTen,
            GioiTinh = bacSi.GioiTinh ?? "Chưa cập nhật",
            NgaySinh = bacSi.NgaySinh,
            TrinhDo = bacSi.TrinhDo ?? "Chưa cập nhật",
            ChuyenKhoa = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật",
            DienThoai = bacSi.DienThoai,
            DiaChi = bacSi.DiaChi,
            MaNguoiDung = nguoiDung.MaNguoiDung,
            TenDangNhap = nguoiDung.TenDangNhap ?? "Chưa cập nhật",
            VaiTro = nguoiDung.VaiTro ?? "Chưa cập nhật",
            TrangThaiTaiKhoan = nguoiDung.TrangThai == true ? "Đang hoạt động" : "Đã khóa",
            SuccessMessage = TempData["CaiDatBacSiSuccess"] as string,
            ErrorMessage = TempData["CaiDatBacSiError"] as string
        };
    }

    private IQueryable<BenhNhan> GetCurrentPatientQuery()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung") ?? string.Empty;
        return _context.BenhNhans.Where(x => x.MaNguoiDung == maNguoiDung);
    }

    private IQueryable<BacSi> GetCurrentDoctorQuery()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung") ?? string.Empty;
        return _context.BacSis.Where(x => x.MaNguoiDung == maNguoiDung);
    }

    private async Task<(bool Success, string Message)> ChangePasswordForCurrentUserAsync(DoiMatKhauViewModel model)
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
        if (nguoiDung == null)
        {
            return (false, "Không tìm thấy tài khoản hiện tại.");
        }

        if (nguoiDung.MatKhau != HashPassword(model.MatKhauHienTai))
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }

        if (model.MatKhauHienTai == model.MatKhauMoi)
        {
            return (false, "Mật khẩu mới cần khác mật khẩu hiện tại.");
        }

        nguoiDung.MatKhau = HashPassword(model.MatKhauMoi);
        await _context.SaveChangesAsync();
        return (true, "Đổi mật khẩu thành công.");
    }

    private IActionResult? RequireRole(string role)
    {
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(currentRole))
        {
            return RedirectToAction("Login", "Account");
        }

        if (currentRole == role)
        {
            return null;
        }

        return currentRole switch
        {
            RolePatient => RedirectToAction(nameof(Index)),
            RoleDoctor => RedirectToAction(nameof(BacSi)),
            _ => RedirectToAction("Login", "Account")
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
