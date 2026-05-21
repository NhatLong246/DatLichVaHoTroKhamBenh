using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class KhamBenhController : Controller
{
    private const string TrangThaiChoKham = "Chờ khám";
    private const string TrangThaiDangKham = "Đang khám";
    private const string TrangThaiDaKham = "Đã khám";
    private const string TrangThaiHuy = "Hủy";
    private const string TrangThaiPhieuDangXuLy = "Đang xử lý";
    private const string TrangThaiPhieuHoanThanh = "Hoàn thành";
    private const string TrangThaiDonThuocChoCap = "Chờ cấp thuốc";
    private const string TrangThaiHoaDonChuaThanhToan = "Chưa thanh toán";
    private const string TrangThaiDichVuHoatDong = "Hoạt động";

    private readonly ApplicationDbContext _context;

    public KhamBenhController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? maDangKy)
    {
        var redirect = RequireDoctorRole();
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildModelAsync(maDangKy);
        if (model == null)
        {
            TempData["KhamBenhError"] = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại.";
            return RedirectToAction("BacSi", "Dashboard");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TiepNhan(string maDangKy)
    {
        var redirect = RequireDoctorRole();
        if (redirect != null)
        {
            return redirect;
        }

        var bacSi = await GetCurrentDoctorAsync();
        if (bacSi == null)
        {
            TempData["KhamBenhError"] = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại.";
            return RedirectToAction("BacSi", "Dashboard");
        }

        var lichKham = await _context.DangKyLichKhams
            .Include(x => x.PhieuKhams)
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy && x.MaBacSi == bacSi.MaBacSi);

        if (lichKham == null)
        {
            TempData["KhamBenhError"] = "Không tìm thấy lịch khám thuộc bác sĩ hiện tại.";
            return RedirectToAction(nameof(Index));
        }

        if (lichKham.TrangThai == TrangThaiHuy || lichKham.TrangThai == TrangThaiDaKham)
        {
            TempData["KhamBenhError"] = "Chỉ có thể tiếp nhận lịch đang chờ khám.";
            return RedirectToAction(nameof(Index), new { maDangKy });
        }

        if (lichKham.TrangThai == TrangThaiChoKham)
        {
            lichKham.TrangThai = TrangThaiDangKham;
        }

        if (!lichKham.PhieuKhams.Any())
        {
            _context.PhieuKhams.Add(new PhieuKham
            {
                MaPhieuKham = await GenerateIdAsync(_context.PhieuKhams.Select(x => x.MaPhieuKham), "PK"),
                MaDangKy = lichKham.MaDangKy,
                TrangThai = TrangThaiPhieuDangXuLy
            });
        }

        await _context.SaveChangesAsync();
        TempData["KhamBenhSuccess"] = "Đã tiếp nhận bệnh nhân vào ca khám.";
        return RedirectToAction(nameof(Index), new { maDangKy });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HoanThanh(KhamBenhViewModel model)
    {
        var redirect = RequireDoctorRole();
        if (redirect != null)
        {
            return redirect;
        }

        if (string.IsNullOrWhiteSpace(model.SelectedMaDangKy))
        {
            TempData["KhamBenhError"] = "Vui lòng chọn bệnh nhân cần khám.";
            return RedirectToAction(nameof(Index));
        }

        ValidateExamination(model);

        var bacSi = await GetCurrentDoctorAsync();
        if (bacSi == null)
        {
            TempData["KhamBenhError"] = "Không tìm thấy hồ sơ bác sĩ cho tài khoản hiện tại.";
            return RedirectToAction("BacSi", "Dashboard");
        }

        var lichKham = await _context.DangKyLichKhams
            .Include(x => x.MaBenhNhanNavigation)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.ChiTietDichVuKhams)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.DonThuocs)
                    .ThenInclude(x => x.ChiTietDonThuocs)
            .FirstOrDefaultAsync(x => x.MaDangKy == model.SelectedMaDangKy && x.MaBacSi == bacSi.MaBacSi);

        if (lichKham == null)
        {
            TempData["KhamBenhError"] = "Không tìm thấy lịch khám thuộc bác sĩ hiện tại.";
            return RedirectToAction(nameof(Index));
        }

        if (lichKham.TrangThai == TrangThaiHuy)
        {
            TempData["KhamBenhError"] = "Không thể hoàn thành lịch khám đã hủy.";
            return RedirectToAction(nameof(Index), new { maDangKy = model.SelectedMaDangKy });
        }

        if (lichKham.TrangThai == TrangThaiDaKham)
        {
            TempData["KhamBenhError"] = "Lịch khám này đã được hoàn thành trước đó.";
            return RedirectToAction(nameof(Index), new { maDangKy = model.SelectedMaDangKy });
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildModelAsync(model.SelectedMaDangKy, model);
            return View(nameof(Index), invalidModel ?? model);
        }

        var phieuKham = lichKham.PhieuKhams.FirstOrDefault();
        if (phieuKham == null)
        {
            phieuKham = new PhieuKham
            {
                MaPhieuKham = await GenerateIdAsync(_context.PhieuKhams.Select(x => x.MaPhieuKham), "PK"),
                MaDangKy = lichKham.MaDangKy
            };
            _context.PhieuKhams.Add(phieuKham);
        }

        phieuKham.TrieuChung = model.TrieuChung?.Trim();
        phieuKham.ChanDoan = model.ChanDoan?.Trim();
        phieuKham.HuongDieuTri = model.HuongDieuTri?.Trim();
        phieuKham.TrangThai = TrangThaiPhieuHoanThanh;

        var oldDichVu = phieuKham.ChiTietDichVuKhams.ToList();
        if (oldDichVu.Count > 0)
        {
            _context.ChiTietDichVuKhams.RemoveRange(oldDichVu);
        }

        var selectedServices = await BuildSelectedServicesAsync(model);
        foreach (var service in selectedServices)
        {
            _context.ChiTietDichVuKhams.Add(new ChiTietDichVuKham
            {
                MaPhieuKham = phieuKham.MaPhieuKham,
                MaDichVu = service.MaDichVu,
                SoLuong = service.SoLuong,
                DonGia = service.DonGia
            });
        }

        foreach (var donThuoc in phieuKham.DonThuocs.ToList())
        {
            _context.ChiTietDonThuocs.RemoveRange(donThuoc.ChiTietDonThuocs);
            _context.DonThuocs.Remove(donThuoc);
        }

        var selectedMedicines = await BuildSelectedMedicinesAsync(model);
        DonThuoc? newDonThuoc = null;
        if (selectedMedicines.Count > 0)
        {
            newDonThuoc = new DonThuoc
            {
                MaDonThuoc = await GenerateIdAsync(_context.DonThuocs.Select(x => x.MaDonThuoc), "DT"),
                MaPhieuKham = phieuKham.MaPhieuKham,
                NgayLap = DateOnly.FromDateTime(DateTime.Now),
                GhiChu = string.IsNullOrWhiteSpace(model.GhiChuDonThuoc)
                    ? "Đã kê đơn. Thuốc không tính vào hóa đơn thanh toán."
                    : model.GhiChuDonThuoc.Trim(),
                TrangThai = TrangThaiDonThuocChoCap
            };
            _context.DonThuocs.Add(newDonThuoc);

            foreach (var medicine in selectedMedicines)
            {
                _context.ChiTietDonThuocs.Add(new ChiTietDonThuoc
                {
                    MaDonThuoc = newDonThuoc.MaDonThuoc,
                    MaThuoc = medicine.MaThuoc,
                    SoLuong = medicine.SoLuong,
                    LieuLuong = medicine.LieuLuong!,
                    SoNgayDung = medicine.SoNgayDung,
                    CachDung = medicine.CachDung!,
                    DonGia = medicine.DonGia
                });
            }
        }

        lichKham.TrangThai = TrangThaiDaKham;

        var tienDichVu = selectedServices.Sum(x => x.DonGia * x.SoLuong);
        var hoaDon = new HoaDon
        {
            MaHoaDon = await GenerateIdAsync(_context.HoaDons.Select(x => x.MaHoaDon), "HD"),
            MaBenhNhan = lichKham.MaBenhNhan,
            NgayLap = DateOnly.FromDateTime(DateTime.Now),
            TongTien = tienDichVu,
            TrangThai = TrangThaiHoaDonChuaThanhToan
        };
        _context.HoaDons.Add(hoaDon);

        _context.ChiTietHoaDons.Add(new ChiTietHoaDon
        {
            MaHoaDon = hoaDon.MaHoaDon,
            MaPhieuKham = phieuKham.MaPhieuKham,
            MaDonThuoc = null,
            TienKham = tienDichVu,
            TienThuoc = 0,
            GhiChu = newDonThuoc == null
                ? "Hóa đơn chỉ gồm tiền khám và dịch vụ đã chỉ định."
                : $"Hóa đơn chỉ gồm tiền khám và dịch vụ. Đơn thuốc {newDonThuoc.MaDonThuoc} đã kê riêng."
        });

        await _context.SaveChangesAsync();

        TempData["KhamBenhSuccess"] = $"Đã hoàn thành ca khám và tạo hóa đơn {hoaDon.MaHoaDon}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<KhamBenhViewModel?> BuildModelAsync(string? maDangKy = null, KhamBenhViewModel? postedModel = null)
    {
        var bacSi = await GetCurrentDoctorAsync();
        if (bacSi == null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        var appointments = await _context.DangKyLichKhams
            .AsNoTracking()
            .Include(x => x.MaBenhNhanNavigation)
            .Include(x => x.MaPhongKhamNavigation)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.ChiTietDichVuKhams)
            .Include(x => x.PhieuKhams)
                .ThenInclude(x => x.DonThuocs)
                    .ThenInclude(x => x.ChiTietDonThuocs)
            .Where(x => x.MaBacSi == bacSi.MaBacSi && x.NgayKham == today && x.TrangThai != TrangThaiHuy)
            .OrderBy(x => x.GioKham ?? TimeOnly.MinValue)
            .ThenBy(x => x.MaDangKy)
            .ToListAsync();

        var selectedMaDangKy = maDangKy
            ?? appointments.FirstOrDefault(x => x.TrangThai == TrangThaiDangKham)?.MaDangKy
            ?? appointments.FirstOrDefault(x => x.TrangThai == TrangThaiChoKham)?.MaDangKy
            ?? appointments.FirstOrDefault()?.MaDangKy;

        var selectedAppointment = appointments.FirstOrDefault(x => x.MaDangKy == selectedMaDangKy);
        var selectedPhieu = selectedAppointment?.PhieuKhams.FirstOrDefault();

        var model = new KhamBenhViewModel
        {
            HoTenBacSi = bacSi.HoTen,
            TenChuyenKhoa = bacSi.MaChuyenKhoaNavigation?.TenChuyenKhoa ?? "Chưa cập nhật chuyên khoa",
            NgayKham = today,
            SelectedMaDangKy = selectedAppointment?.MaDangKy,
            SelectedMaPhieuKham = selectedPhieu?.MaPhieuKham,
            DangChoKham = appointments.Count(x => x.TrangThai == TrangThaiChoKham),
            DangKham = appointments.Count(x => x.TrangThai == TrangThaiDangKham),
            DaHoanThanh = appointments.Count(x => x.TrangThai == TrangThaiDaKham),
            TrieuChung = postedModel?.TrieuChung ?? selectedPhieu?.TrieuChung,
            ChanDoan = postedModel?.ChanDoan ?? selectedPhieu?.ChanDoan,
            HuongDieuTri = postedModel?.HuongDieuTri ?? selectedPhieu?.HuongDieuTri,
            GhiChuDonThuoc = postedModel?.GhiChuDonThuoc ?? selectedPhieu?.DonThuocs.FirstOrDefault()?.GhiChu,
            SuccessMessage = TempData["KhamBenhSuccess"] as string,
            ErrorMessage = TempData["KhamBenhError"] as string,
            HangCho = appointments.Select(x => new KhamBenhQueueItemViewModel
            {
                MaDangKy = x.MaDangKy,
                TenBenhNhan = x.MaBenhNhanNavigation.HoTen,
                DienThoai = x.MaBenhNhanNavigation.DienThoai ?? "Chưa cập nhật",
                GioKhamText = x.GioKham?.ToString("HH:mm") ?? "Chưa hẹn giờ",
                CaKham = x.CaKham,
                TenPhongKham = x.MaPhongKhamNavigation.TenPhongKham,
                TrangThai = x.TrangThai ?? TrangThaiChoKham,
                IsSelected = x.MaDangKy == selectedAppointment?.MaDangKy
            }).ToList()
        };

        if (selectedAppointment != null)
        {
            var patient = selectedAppointment.MaBenhNhanNavigation;
            model.BenhNhanDangChon = new KhamBenhPatientViewModel
            {
                MaDangKy = selectedAppointment.MaDangKy,
                MaBenhNhan = patient.MaBenhNhan,
                HoTen = patient.HoTen,
                DienThoai = patient.DienThoai ?? "Chưa cập nhật",
                GioiTinh = patient.GioiTinh ?? "Chưa cập nhật",
                NgaySinhText = patient.NgaySinh?.ToString("dd/MM/yyyy") ?? "Chưa cập nhật",
                DiaChi = patient.DiaChi ?? "Chưa cập nhật",
                CaKham = selectedAppointment.CaKham,
                GioKhamText = selectedAppointment.GioKham?.ToString("HH:mm") ?? "Chưa hẹn giờ",
                TenPhongKham = selectedAppointment.MaPhongKhamNavigation.TenPhongKham,
                TrangThai = selectedAppointment.TrangThai ?? TrangThaiChoKham
            };
        }

        model.DichVus = await BuildServiceInputsAsync(bacSi.MaChuyenKhoa, selectedPhieu, postedModel);
        model.Thuocs = await BuildMedicineInputsAsync(selectedPhieu, postedModel);

        return model;
    }

    private async Task<List<ChiDinhDichVuInputViewModel>> BuildServiceInputsAsync(
        string? maChuyenKhoa,
        PhieuKham? selectedPhieu,
        KhamBenhViewModel? postedModel)
    {
        var existing = selectedPhieu?.ChiTietDichVuKhams.ToDictionary(x => x.MaDichVu) ?? new();
        var posted = postedModel?.DichVus.ToDictionary(x => x.MaDichVu) ?? new();

        var services = await _context.DichVuKhams
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiDichVuHoatDong && (maChuyenKhoa == null || x.MaChuyenKhoa == maChuyenKhoa))
            .OrderBy(x => x.TenDichVu)
            .ToListAsync();

        return services.Select(x =>
        {
            posted.TryGetValue(x.MaDichVu, out var postedItem);
            existing.TryGetValue(x.MaDichVu, out var existingItem);

            return new ChiDinhDichVuInputViewModel
            {
                MaDichVu = x.MaDichVu,
                TenDichVu = x.TenDichVu,
                MoTa = x.MoTa ?? "Không có mô tả",
                DonGia = x.GiaTien,
                ThoiGianTrungBinh = x.ThoiGianTrungBinh,
                IsSelected = postedItem?.IsSelected ?? existingItem != null,
                SoLuong = postedItem?.SoLuong ?? existingItem?.SoLuong ?? 1
            };
        }).ToList();
    }

    private async Task<List<KeDonThuocInputViewModel>> BuildMedicineInputsAsync(
        PhieuKham? selectedPhieu,
        KhamBenhViewModel? postedModel)
    {
        var existing = selectedPhieu?.DonThuocs
            .SelectMany(x => x.ChiTietDonThuocs)
            .ToDictionary(x => x.MaThuoc) ?? new();
        var posted = postedModel?.Thuocs.ToDictionary(x => x.MaThuoc) ?? new();

        var medicines = await _context.Thuocs
            .AsNoTracking()
            .OrderBy(x => x.TenThuoc)
            .ToListAsync();

        return medicines.Select(x =>
        {
            posted.TryGetValue(x.MaThuoc, out var postedItem);
            existing.TryGetValue(x.MaThuoc, out var existingItem);

            return new KeDonThuocInputViewModel
            {
                MaThuoc = x.MaThuoc,
                TenThuoc = x.TenThuoc,
                DonViTinh = x.DonViTinh,
                SoLuongTon = x.SoLuongTon ?? 0,
                DonGia = x.GiaBan,
                IsSelected = postedItem?.IsSelected ?? existingItem != null,
                SoLuong = postedItem?.SoLuong ?? existingItem?.SoLuong ?? 1,
                LieuLuong = postedItem?.LieuLuong ?? existingItem?.LieuLuong,
                SoNgayDung = postedItem?.SoNgayDung ?? existingItem?.SoNgayDung ?? 1,
                CachDung = postedItem?.CachDung ?? existingItem?.CachDung
            };
        }).ToList();
    }

    private void ValidateExamination(KhamBenhViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.TrieuChung))
        {
            ModelState.AddModelError(nameof(model.TrieuChung), "Vui lòng nhập triệu chứng.");
        }

        if (string.IsNullOrWhiteSpace(model.ChanDoan))
        {
            ModelState.AddModelError(nameof(model.ChanDoan), "Vui lòng nhập chẩn đoán.");
        }

        if (string.IsNullOrWhiteSpace(model.HuongDieuTri))
        {
            ModelState.AddModelError(nameof(model.HuongDieuTri), "Vui lòng nhập hướng điều trị.");
        }

        for (var i = 0; i < model.Thuocs.Count; i++)
        {
            if (!model.Thuocs[i].IsSelected)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(model.Thuocs[i].LieuLuong))
            {
                ModelState.AddModelError($"Thuocs[{i}].LieuLuong", "Vui lòng nhập liều lượng.");
            }

            if (string.IsNullOrWhiteSpace(model.Thuocs[i].CachDung))
            {
                ModelState.AddModelError($"Thuocs[{i}].CachDung", "Vui lòng nhập cách dùng.");
            }
        }
    }

    private async Task<List<ChiDinhDichVuInputViewModel>> BuildSelectedServicesAsync(KhamBenhViewModel model)
    {
        var selected = model.DichVus
            .Where(x => x.IsSelected)
            .Select(x => new { x.MaDichVu, SoLuong = Math.Max(1, x.SoLuong) })
            .ToList();

        if (selected.Count == 0)
        {
            return new List<ChiDinhDichVuInputViewModel>();
        }

        var serviceIds = selected.Select(x => x.MaDichVu).ToList();
        var services = await _context.DichVuKhams
            .AsNoTracking()
            .Where(x => serviceIds.Contains(x.MaDichVu) && x.TrangThai == TrangThaiDichVuHoatDong)
            .ToDictionaryAsync(x => x.MaDichVu);

        return selected
            .Where(x => services.ContainsKey(x.MaDichVu))
            .Select(x => new ChiDinhDichVuInputViewModel
            {
                MaDichVu = x.MaDichVu,
                DonGia = services[x.MaDichVu].GiaTien,
                SoLuong = x.SoLuong
            })
            .ToList();
    }

    private async Task<List<KeDonThuocInputViewModel>> BuildSelectedMedicinesAsync(KhamBenhViewModel model)
    {
        var selected = model.Thuocs
            .Where(x => x.IsSelected)
            .Select(x => new
            {
                x.MaThuoc,
                SoLuong = Math.Max(1, x.SoLuong),
                LieuLuong = x.LieuLuong?.Trim(),
                SoNgayDung = Math.Max(1, x.SoNgayDung),
                CachDung = x.CachDung?.Trim()
            })
            .ToList();

        if (selected.Count == 0)
        {
            return new List<KeDonThuocInputViewModel>();
        }

        var medicineIds = selected.Select(x => x.MaThuoc).ToList();
        var medicines = await _context.Thuocs
            .AsNoTracking()
            .Where(x => medicineIds.Contains(x.MaThuoc))
            .ToDictionaryAsync(x => x.MaThuoc);

        return selected
            .Where(x => medicines.ContainsKey(x.MaThuoc))
            .Select(x => new KeDonThuocInputViewModel
            {
                MaThuoc = x.MaThuoc,
                DonGia = medicines[x.MaThuoc].GiaBan,
                SoLuong = x.SoLuong,
                LieuLuong = x.LieuLuong,
                SoNgayDung = x.SoNgayDung,
                CachDung = x.CachDung
            })
            .ToList();
    }

    private async Task<BacSi?> GetCurrentDoctorAsync()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        if (string.IsNullOrWhiteSpace(maNguoiDung))
        {
            return null;
        }

        return await _context.BacSis
            .Include(x => x.MaChuyenKhoaNavigation)
            .FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
    }

    private static async Task<string> GenerateIdAsync(IQueryable<string> source, string prefix)
    {
        var ids = await source.AsNoTracking().ToListAsync();
        var nextNumber = ids
            .Select(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(x[prefix.Length..], out var number) ? number : 0)
            .DefaultIfEmpty()
            .Max() + 1;

        return $"{prefix}{nextNumber:000}";
    }

    private IActionResult? RequireDoctorRole()
    {
        var currentRole = HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(currentRole))
        {
            return RedirectToAction("Login", "Account");
        }

        return currentRole == "Bác sĩ" ? null : RedirectToAction("BacSi", "Dashboard");
    }
}
