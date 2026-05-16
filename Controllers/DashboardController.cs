using HeThongDatLichVaKhamBenh.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> BenhNhan()
    {
        var redirect = RequireRole("Bệnh nhân");
        if (redirect != null)
        {
            return redirect;
        }

        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var benhNhan = await _context.BenhNhans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        ViewBag.HoTen = benhNhan?.HoTen ?? HttpContext.Session.GetString("TenDangNhap") ?? "Bệnh nhân";
        return View();
    }

    public async Task<IActionResult> BacSi()
    {
        var redirect = RequireRole("Bác sĩ");
        if (redirect != null)
        {
            return redirect;
        }

        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var bacSi = await _context.BacSis
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        ViewBag.HoTen = bacSi?.HoTen ?? HttpContext.Session.GetString("TenDangNhap") ?? "Bác sĩ";
        ViewBag.Today = DateTime.Now.ToString("dd/MM/yyyy");
        return View();
    }

    public IActionResult Admin()
    {
        var redirect = RequireRole("Quản trị");
        if (redirect != null)
        {
            return redirect;
        }

        ViewBag.Today = DateTime.Now.ToString("dd/MM/yyyy");
        return View();
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
                "Bệnh nhân" => RedirectToAction(nameof(BenhNhan)),
                "Bác sĩ" => RedirectToAction(nameof(BacSi)),
                "Quản trị" => RedirectToAction(nameof(Admin)),
                _ => RedirectToAction("Login", "Account")
            };
        }

        return null;
    }
}
