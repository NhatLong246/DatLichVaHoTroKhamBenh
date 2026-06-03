using System.Security.Cryptography;
using System.Text;
using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("VaiTro")))
        {
            return RedirectToRoleHome(HttpContext.Session.GetString("VaiTro"));
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var passwordHash = HashPassword(model.MatKhau);
        var user = await _context.NguoiDungs
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenDangNhap == model.TenDangNhap &&
                x.MatKhau == passwordHash);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (user.TrangThai != true)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa hoặc ngừng sử dụng.");
            return View(model);
        }

        HttpContext.Session.SetString("MaNguoiDung", user.MaNguoiDung);
        HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap ?? string.Empty);
        HttpContext.Session.SetString("VaiTro", user.VaiTro ?? string.Empty);

        return RedirectToRoleHome(user.VaiTro);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("VaiTro")))
        {
            return RedirectToRoleHome(HttpContext.Session.GetString("VaiTro"));
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var isUsernameExist = await _context.NguoiDungs.AnyAsync(x => x.TenDangNhap == model.TenDangNhap);
        if (isUsernameExist)
        {
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
            return View(model);
        }

        // Email validation (optional but good practice)
        var isEmailExist = await _context.NguoiDungs.AnyAsync(x => x.Email == model.Email);
        if (isEmailExist)
        {
            ModelState.AddModelError("Email", "Email đã được sử dụng.");
            return View(model);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Generate IDs
            var maxNguoiDungId = await _context.NguoiDungs
                .Select(x => x.MaNguoiDung)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();
            var nextNdNum = maxNguoiDungId != null && int.TryParse(maxNguoiDungId.Substring(2), out int numNd) ? numNd + 1 : 1;
            var newMaNguoiDung = $"ND{nextNdNum:000}";

            var maxBenhNhanId = await _context.BenhNhans
                .Select(x => x.MaBenhNhan)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();
            var nextBnNum = maxBenhNhanId != null && int.TryParse(maxBenhNhanId.Substring(2), out int numBn) ? numBn + 1 : 1;
            var newMaBenhNhan = $"BN{nextBnNum:000}";

            // Insert NguoiDung
            var nguoiDung = new NguoiDung
            {
                MaNguoiDung = newMaNguoiDung,
                TenDangNhap = model.TenDangNhap,
                MatKhau = HashPassword(model.MatKhau),
                VaiTro = "Bệnh nhân",
                TrangThai = true,
                Email = model.Email
            };
            _context.NguoiDungs.Add(nguoiDung);

            // Insert BenhNhan
            var benhNhan = new BenhNhan
            {
                MaBenhNhan = newMaBenhNhan,
                MaNguoiDung = newMaNguoiDung,
                HoTen = model.HoTen,
                GioiTinh = model.GioiTinh,
                NgaySinh = model.NgaySinh,
                DienThoai = model.DienThoai,
                DiaChi = model.DiaChi
            };
            _context.BenhNhans.Add(benhNhan);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["RegisterSuccess"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại sau.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    private RedirectToActionResult RedirectToRoleHome(string? role)
    {
        return role switch
        {
            "Bệnh nhân" => RedirectToAction("BenhNhan", "Dashboard"),
            "Bác sĩ" => RedirectToAction("BacSi", "Dashboard"),
            "Quản trị" => RedirectToAction("Admin", "Dashboard"),
            _ => RedirectToAction(nameof(Login))
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.Unicode.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
