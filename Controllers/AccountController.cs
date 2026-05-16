using System.Security.Cryptography;
using System.Text;
using HeThongDatLichVaKhamBenh.Models.EF;
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
