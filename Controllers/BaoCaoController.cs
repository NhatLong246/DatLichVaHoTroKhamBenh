using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class BaoCaoController : Controller
{
    private readonly ApplicationDbContext _context;

    public BaoCaoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(BaoCaoFilterViewModel filter)
    {
        // Require Admin role
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (currentRole != "Quản trị")
        {
            return RedirectToAction("Login", "Account");
        }

        // Set default filter if empty
        if (!filter.TuNgay.HasValue && !filter.DenNgay.HasValue && string.IsNullOrEmpty(filter.MaChuyenKhoa) && string.IsNullOrEmpty(filter.MaBacSi))
        {
            filter.TuNgay = DateTime.Now.Date.AddDays(-30);
            filter.DenNgay = DateTime.Now.Date;
        }

        var hoaDonQuery = _context.HoaDons
            .Include(h => h.MaBenhNhanNavigation)
            .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.MaPhieuKhamNavigation)
                    .ThenInclude(pk => pk.MaDangKyNavigation)
            .Where(h => h.TrangThai == "Đã thanh toán");

        var lichKhamQuery = _context.DangKyLichKhams
            .Include(d => d.MaBacSiNavigation)
                .ThenInclude(b => b.MaChuyenKhoaNavigation)
            .Where(d => d.TrangThai == "Đã khám" || d.TrangThai == "Đang khám");

        if (filter.TuNgay.HasValue)
        {
            var tuNgayDateOnly = DateOnly.FromDateTime(filter.TuNgay.Value.Date);
            hoaDonQuery = hoaDonQuery.Where(h => h.NgayLap != null && h.NgayLap >= tuNgayDateOnly);
            lichKhamQuery = lichKhamQuery.Where(d => d.NgayKham >= tuNgayDateOnly);
        }

        if (filter.DenNgay.HasValue)
        {
            var denNgayDateOnly = DateOnly.FromDateTime(filter.DenNgay.Value.Date);
            hoaDonQuery = hoaDonQuery.Where(h => h.NgayLap != null && h.NgayLap <= denNgayDateOnly);
            lichKhamQuery = lichKhamQuery.Where(d => d.NgayKham <= denNgayDateOnly);
        }

        if (!string.IsNullOrEmpty(filter.MaBacSi))
        {
            hoaDonQuery = hoaDonQuery.Where(h => h.ChiTietHoaDons.Any(ct => ct.MaPhieuKhamNavigation.MaDangKyNavigation.MaBacSi == filter.MaBacSi));
            lichKhamQuery = lichKhamQuery.Where(d => d.MaBacSi == filter.MaBacSi);
        }

        if (!string.IsNullOrEmpty(filter.MaChuyenKhoa))
        {
            hoaDonQuery = hoaDonQuery.Where(h => h.ChiTietHoaDons.Any(ct => ct.MaPhieuKhamNavigation.MaDangKyNavigation.MaBacSiNavigation.MaChuyenKhoa == filter.MaChuyenKhoa));
            lichKhamQuery = lichKhamQuery.Where(d => d.MaBacSiNavigation.MaChuyenKhoa == filter.MaChuyenKhoa);
        }

        var hoaDons = await hoaDonQuery.OrderByDescending(x => x.NgayLap).ToListAsync();
        var lichKhams = await lichKhamQuery.ToListAsync();

        var model = new BaoCaoViewModel
        {
            Filter = filter,
            TongDoanhThu = hoaDons.Sum(x => x.TongTien ?? 0),
            TongHoaDon = hoaDons.Count,
            TongLuotKham = lichKhams.Count,
            DanhSachHoaDon = hoaDons.Take(50).Select(x => new BaoCaoHoaDonItem
            {
                MaHoaDon = x.MaHoaDon,
                TenBenhNhan = x.MaBenhNhanNavigation != null ? x.MaBenhNhanNavigation.HoTen : "",
                NgayLap = x.NgayLap ?? DateOnly.MinValue,
                TongTien = x.TongTien ?? 0
            }).ToList()
        };

        var colors = new[] { "#3b82f6", "#10b981", "#f59e0b", "#8b5cf6", "#ec4899", "#ef4444", "#14b8a6", "#6366f1" };
        
        model.ChartChuyenKhoa = lichKhams
            .GroupBy(x => x.MaBacSiNavigation?.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Không rõ")
            .Select((g, index) => new BaoCaoChuyenKhoaChartItem
            {
                TenChuyenKhoa = g.Key,
                SoLuotKham = g.Count(),
                Color = colors[index % colors.Length]
            })
            .OrderByDescending(x => x.SoLuotKham)
            .ToList();

        // Calculate revenue per doctor based on the retrieved invoices
        var doanhThuBacSi = new Dictionary<string, decimal>();
        foreach (var hd in hoaDons)
        {
            var firstDoc = hd.ChiTietHoaDons.FirstOrDefault()?.MaPhieuKhamNavigation.MaDangKyNavigation.MaBacSi;
            if (firstDoc != null)
            {
                if (!doanhThuBacSi.ContainsKey(firstDoc))
                {
                    doanhThuBacSi[firstDoc] = 0;
                }
                doanhThuBacSi[firstDoc] += hd.TongTien ?? 0;
            }
        }

        model.ThongKeBacSi = lichKhams
            .GroupBy(x => new { x.MaBacSi, x.MaBacSiNavigation?.HoTen, TenChuyenKhoa = x.MaBacSiNavigation?.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Không rõ" })
            .Select(g => new BaoCaoBacSiItem
            {
                MaBacSi = g.Key.MaBacSi,
                TenBacSi = g.Key.HoTen,
                TenChuyenKhoa = g.Key.TenChuyenKhoa,
                SoLuotKham = g.Count(),
                DoanhThuMangLai = doanhThuBacSi.ContainsKey(g.Key.MaBacSi) ? doanhThuBacSi[g.Key.MaBacSi] : 0
            })
            .OrderByDescending(x => x.SoLuotKham)
            .ToList();

        // Load dropdowns
        var chuyenKhoas = await _context.ChuyenKhoas.OrderBy(x => x.TenChuyenKhoa).ToListAsync();
        model.DanhSachChuyenKhoa = chuyenKhoas.Select(x => new SelectListItem { Value = x.MaChuyenKhoa, Text = x.TenChuyenKhoa }).ToList();

        var bacSis = await _context.BacSis.OrderBy(x => x.HoTen).ToListAsync();
        model.DanhSachBacSi = bacSis.Select(x => new SelectListItem { Value = x.MaBacSi, Text = x.HoTen }).ToList();

        return View(model);
    }
}
