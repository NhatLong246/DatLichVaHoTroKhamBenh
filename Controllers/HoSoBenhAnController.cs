using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class HoSoBenhAnController : Controller
{
    private readonly ApplicationDbContext _context;

    public HoSoBenhAnController(ApplicationDbContext context)
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

        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        var benhNhan = await _context.BenhNhans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);

        if (benhNhan == null)
        {
            return RedirectToAction("BenhNhan", "Dashboard");
        }

        var lichKhams = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBacSiNavigation)
                .ThenInclude(x => x.MaChuyenKhoaNavigation)
            .Include(x => x.MaPhongKhamNavigation)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.DonThuocs)
                    .ThenInclude(x => x.ChiTietDonThuocs)
                        .ThenInclude(x => x.MaThuocNavigation)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.ChiTietDichVuKhams)
                    .ThenInclude(x => x.MaDichVuNavigation)
            .Where(x => x.MaBenhNhan == benhNhan.MaBenhNhan && x.PhieuKhams.Any())
            .OrderByDescending(x => x.NgayKham)
            .ThenByDescending(x => x.GioKham)
            .ToListAsync();

        var lanKhams = lichKhams
            .SelectMany(lich => lich.PhieuKhams.Select(phieu => new LanKhamViewModel
            {
                MaDangKy = lich.MaDangKy,
                MaPhieuKham = phieu.MaPhieuKham,
                NgayKham = lich.NgayKham,
                CaKham = lich.CaKham,
                GioKham = lich.GioKham,
                TrangThaiLich = lich.TrangThai ?? "Chưa cập nhật",
                TrangThaiPhieu = phieu.TrangThai ?? "Chưa cập nhật",
                TenBacSi = lich.MaBacSiNavigation.HoTen,
                TrinhDoBacSi = lich.MaBacSiNavigation.TrinhDo ?? "Bác sĩ",
                TenChuyenKhoa = lich.MaBacSiNavigation.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật",
                TenPhongKham = lich.MaPhongKhamNavigation.TenPhongKham,
                ViTriPhongKham = lich.MaPhongKhamNavigation.ViTri ?? string.Empty,
                TrieuChung = phieu.TrieuChung ?? "Chưa cập nhật",
                ChanDoan = phieu.ChanDoan ?? "Chưa cập nhật",
                HuongDieuTri = phieu.HuongDieuTri ?? "Chưa cập nhật",
                DonThuocs = phieu.DonThuocs
                    .OrderByDescending(don => don.NgayLap)
                    .Select(don => new DonThuocHoSoViewModel
                    {
                        MaDonThuoc = don.MaDonThuoc,
                        NgayLap = don.NgayLap,
                        TrangThai = don.TrangThai ?? "Chưa cập nhật",
                        GhiChu = don.GhiChu ?? "Không có ghi chú",
                        Thuocs = don.ChiTietDonThuocs
                            .OrderBy(chiTiet => chiTiet.MaThuocNavigation.TenThuoc)
                            .Select(chiTiet => new ThuocHoSoViewModel
                            {
                                MaThuoc = chiTiet.MaThuoc,
                                TenThuoc = chiTiet.MaThuocNavigation.TenThuoc,
                                DonViTinh = chiTiet.MaThuocNavigation.DonViTinh,
                                SoLuong = chiTiet.SoLuong,
                                LieuLuong = chiTiet.LieuLuong,
                                SoNgayDung = chiTiet.SoNgayDung,
                                CachDung = chiTiet.CachDung,
                                DonGia = chiTiet.DonGia,
                                ThanhTien = chiTiet.ThanhTien ?? chiTiet.SoLuong * chiTiet.DonGia
                            })
                            .ToList()
                    })
                    .ToList(),
                DichVuDaDung = phieu.ChiTietDichVuKhams
                    .OrderBy(dichVu => dichVu.MaDichVuNavigation.TenDichVu)
                    .Select(dichVu => new DichVuHoSoViewModel
                    {
                        MaDichVu = dichVu.MaDichVu,
                        TenDichVu = dichVu.MaDichVuNavigation.TenDichVu,
                        MoTa = dichVu.MaDichVuNavigation.MoTa ?? string.Empty,
                        SoLuong = dichVu.SoLuong,
                        DonGia = dichVu.DonGia,
                        ThanhTien = dichVu.ThanhTien ?? dichVu.SoLuong * dichVu.DonGia
                    })
                    .ToList()
            }))
            .OrderByDescending(x => x.NgayKham)
            .ThenByDescending(x => x.GioKham)
            .ToList();

        var model = new HoSoBenhAnViewModel
        {
            HoTenBenhNhan = benhNhan.HoTen,
            TongLanKham = lanKhams.Count,
            TongDonThuoc = lanKhams.Sum(x => x.DonThuocs.Count),
            TongDichVu = lanKhams.Sum(x => x.DichVuDaDung.Count),
            TongChiPhiDichVu = lanKhams.Sum(x => x.TongTienDichVu),
            LanKhams = lanKhams
        };

        return View(model);
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
