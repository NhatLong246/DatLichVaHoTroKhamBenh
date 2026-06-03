using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using HeThongDatLichVaKhamBenh.Services;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class HoaDonController : Controller
{
    private static readonly string[] ValidPaymentMethods = ["Tiền mặt", "Thẻ", "Chuyển khoản", "Bảo hiểm"];

    private readonly ApplicationDbContext _context;
    private readonly IMoMoService _momoService;

    public HoaDonController(ApplicationDbContext context, IMoMoService momoService)
    {
        _context = context;
        _momoService = momoService;
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

    [HttpPost]
    public async Task<IActionResult> CreateMoMoPayment(string maHoaDon)
    {
        var redirect = RequirePatientRole();
        if (redirect != null) return Json(new { success = false, message = "Vui lòng đăng nhập." });

        var benhNhan = await GetCurrentPatientAsync();
        if (benhNhan == null) return Json(new { success = false, message = "Không tìm thấy hồ sơ bệnh nhân." });

        var hoaDon = await _context.HoaDons
            .Include(h => h.ChiTietHoaDons)
            .FirstOrDefaultAsync(x => x.MaHoaDon == maHoaDon && x.MaBenhNhan == benhNhan.MaBenhNhan);

        if (hoaDon == null || hoaDon.TrangThai != "Chưa thanh toán")
        {
            return Json(new { success = false, message = "Hóa đơn không tồn tại hoặc đã thanh toán." });
        }

        long totalAmount = (long)(hoaDon.TongTien ?? hoaDon.ChiTietHoaDons.Sum(x => (x.TienKham ?? 0) + (x.TienThuoc ?? 0)));
        if (totalAmount <= 0)
        {
            return Json(new { success = false, message = "Số tiền thanh toán không hợp lệ." });
        }

        string orderId = $"{hoaDon.MaHoaDon}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        string orderInfo = $"Thanh toan hoa don kham benh {hoaDon.MaHoaDon}";

        var response = await _momoService.CreatePaymentAsync(orderId, orderInfo, totalAmount);
        
        if (response != null && response.resultCode == 0)
        {
            // Use deeplink for QR generation if available. This forces the MoMo app to open the native 
            // payment screen instead of a webview when scanned. If deeplink is empty, fallback to payUrl.
            string qrContent = !string.IsNullOrEmpty(response.deeplink) ? response.deeplink : (response.payUrl ?? "");
            string finalQrUrl = $"https://quickchart.io/qr?size=300&text={Uri.EscapeDataString(qrContent)}";
            
            return Json(new { success = true, qrCodeUrl = finalQrUrl, orderId = orderId });
        }

        string errorMsg = response?.message ?? "Không thể tạo mã thanh toán từ MoMo lúc này. Hãy thử lại sau.";
        return Json(new { success = false, message = $"Lỗi MoMo: {errorMsg}" });
    }

    [HttpPost]
    public async Task<IActionResult> CheckMoMoPaymentStatus(string maHoaDon, string orderId)
    {
        var redirect = RequirePatientRole();
        if (redirect != null) return Json(new { success = false });

        var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(x => x.MaHoaDon == maHoaDon);
        if (hoaDon == null) return Json(new { success = false });

        if (hoaDon.TrangThai == "Đã thanh toán") return Json(new { success = true, message = "Đã thanh toán trước đó" });

        var response = await _momoService.QueryPaymentAsync(orderId, Guid.NewGuid().ToString());

        // resultCode == 0 means Transaction Success in MoMo
        if (response != null && response.resultCode == 0)
        {
            hoaDon.TrangThai = "Đã thanh toán";
            hoaDon.HinhThucThanhToan = "Chuyển khoản";
            await _context.SaveChangesAsync();

            TempData["HoaDonSuccess"] = $"Thanh toán hóa đơn {hoaDon.MaHoaDon} qua MoMo thành công.";
            return Json(new { success = true });
        }

        return Json(new { success = false });
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
