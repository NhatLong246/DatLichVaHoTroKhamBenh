using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class QuanLyTaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuanLyTaiKhoanController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string search, string role)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị")
        {
            return RedirectToAction("Login", "Account");
        }

        var query = _context.NguoiDungs
            .Where(x => x.VaiTro != "Quản trị")
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(x => x.VaiTro == role);
        }

        var listNguoiDung = await query.ToListAsync();
        
        var listTaiKhoan = new List<ThongTinTaiKhoanViewModel>();

        foreach (var user in listNguoiDung)
        {
            var item = new ThongTinTaiKhoanViewModel
            {
                MaNguoiDung = user.MaNguoiDung,
                TenDangNhap = user.TenDangNhap ?? "",
                VaiTro = user.VaiTro ?? "",
                TrangThai = user.TrangThai ?? false
            };

            if (user.VaiTro == "Bác sĩ")
            {
                var bacSi = await _context.BacSis
                    .Include(b => b.MaChuyenKhoaNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.MaNguoiDung == user.MaNguoiDung);
                
                if (bacSi != null)
                {
                    item.HoTen = bacSi.HoTen;
                    item.ChuyenKhoaHoacGioiTinh = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? bacSi.GioiTinh ?? "";
                }
            }
            else if (user.VaiTro == "Bệnh nhân")
            {
                var benhNhan = await _context.BenhNhans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.MaNguoiDung == user.MaNguoiDung);
                
                if (benhNhan != null)
                {
                    item.HoTen = benhNhan.HoTen;
                    item.ChuyenKhoaHoacGioiTinh = benhNhan.GioiTinh ?? "";
                }
            }
            
            // Fallback
            if (string.IsNullOrEmpty(item.HoTen))
            {
                item.HoTen = "Chưa cập nhật hồ sơ";
            }

            listTaiKhoan.Add(item);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            listTaiKhoan = listTaiKhoan.Where(x => 
                x.HoTen.ToLower().Contains(search) || 
                x.TenDangNhap.ToLower().Contains(search) ||
                x.MaNguoiDung.ToLower().Contains(search)
            ).ToList();
        }

        var model = new QuanLyTaiKhoanViewModel
        {
            Search = search ?? "",
            Role = role ?? "",
            DanhSachTaiKhoan = listTaiKhoan.OrderBy(x => x.VaiTro).ThenBy(x => x.TenDangNhap).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string maNguoiDung)
    {
        if (HttpContext.Session.GetString("VaiTro") != "Quản trị")
        {
            return Unauthorized();
        }

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
        if (user != null && user.VaiTro != "Quản trị")
        {
            user.TrangThai = !user.TrangThai;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = user.TrangThai == true 
                ? $"Đã mở khóa tài khoản {user.TenDangNhap}"
                : $"Đã khóa tài khoản {user.TenDangNhap}";
        }
        else
        {
            TempData["ErrorMessage"] = "Không tìm thấy người dùng hoặc không có quyền khóa tài khoản này.";
        }

        return RedirectToAction(nameof(Index));
    }
}
