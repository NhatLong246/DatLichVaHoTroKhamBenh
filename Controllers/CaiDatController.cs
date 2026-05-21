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
    private readonly ApplicationDbContext _context;

    public CaiDatController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildSettingsModelAsync();
        if (model == null)
        {
            return RedirectToAction("BenhNhan", "Dashboard");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatThongTin(CaiDatViewModel model)
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        var benhNhan = await GetCurrentPatientQuery()
            .FirstOrDefaultAsync();

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
    public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        if (!ModelState.IsValid)
        {
            TempData["CaiDatError"] = "Vui lòng kiểm tra lại thông tin đổi mật khẩu.";
            return RedirectToAction(nameof(Index));
        }

        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
        if (nguoiDung == null)
        {
            TempData["CaiDatError"] = "Không tìm thấy tài khoản hiện tại.";
            return RedirectToAction("Login", "Account");
        }

        if (nguoiDung.MatKhau != HashPassword(model.MatKhauHienTai))
        {
            TempData["CaiDatError"] = "Mật khẩu hiện tại không đúng.";
            return RedirectToAction(nameof(Index));
        }

        if (model.MatKhauHienTai == model.MatKhauMoi)
        {
            TempData["CaiDatError"] = "Mật khẩu mới cần khác mật khẩu hiện tại.";
            return RedirectToAction(nameof(Index));
        }

        nguoiDung.MatKhau = HashPassword(model.MatKhauMoi);
        await _context.SaveChangesAsync();

        TempData["CaiDatSuccess"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(Index));
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

    private IQueryable<BenhNhan> GetCurrentPatientQuery()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung") ?? string.Empty;
        return _context.BenhNhans.Where(x => x.MaNguoiDung == maNguoiDung);
    }

    private IActionResult? RequirePatientRole()
    {
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(currentRole))
        {
            return RedirectToAction("Login", "Account");
        }

        return currentRole == "Bệnh nhân" ? null : RedirectToAction("BenhNhan", "Dashboard");
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
