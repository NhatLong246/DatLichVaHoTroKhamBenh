using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class HoaDonController : Controller
{
    private static readonly string[] ValidPaymentMethods = ["Tiền mặt", "Thẻ", "Chuyển khoản", "Bảo hiểm"];

    private readonly ApplicationDbContext _context;

    public HoaDonController(ApplicationDbContext context)
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

        var model = await BuildHoaDonModelAsync();
        if (model == null)
        {
            TempData["HoaDonError"] = "Không tìm thấy hồ sơ bệnh nhân cho tài khoản hiện tại.";
            return RedirectToAction("BenhNhan", "Dashboard");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThanhToan(string maHoaDon, string hinhThucThanhToan)
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        if (!ValidPaymentMethods.Contains(hinhThucThanhToan))
        {
            TempData["HoaDonError"] = "Hình thức thanh toán không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        var benhNhan = await GetCurrentPatientAsync();
        if (benhNhan == null)
        {
            TempData["HoaDonError"] = "Không tìm thấy hồ sơ bệnh nhân cho tài khoản hiện tại.";
            return RedirectToAction("BenhNhan", "Dashboard");
        }

        var hoaDon = await _context.HoaDons
            .FirstOrDefaultAsync(x => x.MaHoaDon == maHoaDon && x.MaBenhNhan == benhNhan.MaBenhNhan);

        if (hoaDon == null)
        {
            TempData["HoaDonError"] = "Không tìm thấy hóa đơn cần thanh toán.";
            return RedirectToAction(nameof(Index));
        }

        if (hoaDon.TrangThai != "Chưa thanh toán")
        {
            TempData["HoaDonError"] = "Chỉ có thể thanh toán hóa đơn đang ở trạng thái chưa thanh toán.";
            return RedirectToAction(nameof(Index));
        }

        hoaDon.HinhThucThanhToan = hinhThucThanhToan;
        hoaDon.TrangThai = "Đã thanh toán";
        await _context.SaveChangesAsync();

        TempData["HoaDonSuccess"] = $"Thanh toán hóa đơn {hoaDon.MaHoaDon} thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<HoaDonViewModel?> BuildHoaDonModelAsync()
    {
        var benhNhan = await GetCurrentPatientAsync();
        if (benhNhan == null)
        {
            return null;
        }

        var hoaDons = await _context.HoaDons
            .AsNoTracking()
            .Include(x => x.ChiTietHoaDons)
                .ThenInclude(x => x.MaPhieuKhamNavigation)
                    .ThenInclude(x => x.MaDangKyNavigation)
                        .ThenInclude(x => x.MaBacSiNavigation)
                            .ThenInclude(x => x.MaChuyenKhoaNavigation)
            .Include(x => x.ChiTietHoaDons)
                .ThenInclude(x => x.MaPhieuKhamNavigation)
                    .ThenInclude(x => x.MaDangKyNavigation)
                        .ThenInclude(x => x.MaPhongKhamNavigation)
            .Where(x => x.MaBenhNhan == benhNhan.MaBenhNhan)
            .OrderByDescending(x => x.NgayLap)
            .ThenByDescending(x => x.MaHoaDon)
            .ToListAsync();

        var items = hoaDons.Select(hoaDon => new HoaDonItemViewModel
        {
            MaHoaDon = hoaDon.MaHoaDon,
            NgayLap = hoaDon.NgayLap,
            TongTien = hoaDon.TongTien ?? hoaDon.ChiTietHoaDons.Sum(x => (x.TienKham ?? 0) + (x.TienThuoc ?? 0)),
            HinhThucThanhToan = hoaDon.HinhThucThanhToan ?? "Chưa chọn",
            TrangThai = hoaDon.TrangThai ?? "Chưa thanh toán",
            ChiTietHoaDons = hoaDon.ChiTietHoaDons
                .OrderBy(x => x.MaChiTiet)
                .Select(chiTiet =>
                {
                    var phieuKham = chiTiet.MaPhieuKhamNavigation;
                    var lichKham = phieuKham.MaDangKyNavigation;
                    var bacSi = lichKham.MaBacSiNavigation;
                    var phongKham = lichKham.MaPhongKhamNavigation;

                    return new ChiTietHoaDonViewModel
                    {
                        MaChiTiet = chiTiet.MaChiTiet,
                        MaPhieuKham = chiTiet.MaPhieuKham,
                        MaDonThuoc = chiTiet.MaDonThuoc ?? "Không có",
                        MaDangKy = lichKham.MaDangKy,
                        NgayKham = lichKham.NgayKham,
                        CaKham = lichKham.CaKham,
                        TenBacSi = bacSi.HoTen,
                        TenChuyenKhoa = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật",
                        TenPhongKham = phongKham.TenPhongKham,
                        TienKham = chiTiet.TienKham ?? 0,
                        TienThuoc = chiTiet.TienThuoc ?? 0,
                        GhiChu = chiTiet.GhiChu ?? "Không có ghi chú"
                    };
                })
                .ToList()
        }).ToList();

        return new HoaDonViewModel
        {
            HoTenBenhNhan = benhNhan.HoTen,
            TongHoaDon = items.Count,
            ChuaThanhToan = items.Count(x => x.TrangThai == "Chưa thanh toán"),
            DaThanhToan = items.Count(x => x.TrangThai == "Đã thanh toán"),
            TongChiPhi = items.Sum(x => x.TongTien),
            SuccessMessage = TempData["HoaDonSuccess"] as string,
            ErrorMessage = TempData["HoaDonError"] as string,
            HoaDons = items
        };
    }

    private async Task<Models.Entities.BenhNhan?> GetCurrentPatientAsync()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        if (string.IsNullOrWhiteSpace(maNguoiDung))
        {
            return null;
        }

        return await _context.BenhNhans.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
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
}
