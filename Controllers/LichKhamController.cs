using System.Globalization;
using System.Text;
using HeThongDatLichVaKhamBenh.Models.EF;
using HeThongDatLichVaKhamBenh.Models.Entities;
using HeThongDatLichVaKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Controllers;

public class LichKhamController : Controller
{
    private static readonly string[] ValidShifts = ["Sáng", "Chiều", "Tối"];
    private static readonly List<SymptomRule> SymptomRules =
    [
        new("Tim mạch", 45, "đau ngực", "tức ngực", "khó thở", "hồi hộp", "tim đập nhanh", "mệt khi gắng sức", "phù chân", "choáng váng"),
        new("Tiêu hóa", 40, "đau bụng", "buồn nôn", "nôn", "tiêu chảy", "táo bón", "ợ nóng", "đầy hơi", "khó tiêu", "đau dạ dày", "đi ngoài"),
        new("Tai Mũi Họng", 30, "đau họng", "ho", "nghẹt mũi", "sổ mũi", "ù tai", "đau tai", "khàn tiếng", "chảy máu mũi", "viêm xoang"),
        new("Mắt", 35, "mờ mắt", "đau mắt", "đỏ mắt", "chảy nước mắt", "cộm mắt", "ngứa mắt", "nhìn đôi", "giảm thị lực"),
        new("Da liễu", 30, "ngứa", "nổi mẩn", "phát ban", "mụn", "mụn nước", "bong tróc da", "dị ứng da", "nấm da", "mề đay"),
        new("Thần kinh", 45, "đau đầu", "chóng mặt", "tê tay", "tê chân", "co giật", "mất ngủ", "run tay", "đau nửa đầu", "yếu liệt"),
        new("Sản phụ khoa", 40, "đau bụng dưới", "rối loạn kinh nguyệt", "khí hư", "đau khi quan hệ", "mang thai", "khám thai", "rong kinh", "chậm kinh"),
        new("Nhi khoa", 30, "trẻ", "bé", "em bé", "sốt ở trẻ", "trẻ ho", "trẻ nôn", "trẻ tiêu chảy", "quấy khóc", "biếng ăn"),
        new("Ngoại khoa", 35, "vết thương", "chấn thương", "đau sau té", "sưng đau", "gãy xương", "bong gân", "áp xe", "đau khớp", "u cục"),
        new("Nội khoa", 30, "sốt", "mệt mỏi", "ớn lạnh", "đau nhức toàn thân", "cao huyết áp", "tiểu nhiều", "sụt cân", "kiểm tra sức khỏe")
    ];

    private static readonly string[] UrgentKeywords =
    [
        "đau ngực dữ dội", "khó thở nặng", "ngất", "liệt nửa người", "co giật kéo dài",
        "chảy máu nhiều", "sốt cao không hạ", "đau đầu dữ dội", "nói khó", "méo miệng"
    ];

    private static readonly Dictionary<string, ShiftTimeRange> ShiftTimeRanges = new()
    {
        ["Sáng"] = new ShiftTimeRange(new TimeOnly(7, 30), new TimeOnly(11, 30)),
        ["Chiều"] = new ShiftTimeRange(new TimeOnly(13, 30), new TimeOnly(17, 0)),
        ["Tối"] = new ShiftTimeRange(new TimeOnly(18, 0), new TimeOnly(20, 30))
    };

    private readonly ApplicationDbContext _context;

    public LichKhamController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> DatLich()
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        var model = await BuildDatLichModelAsync(new DatLichKhamViewModel
        {
            NgayKham = DateTime.Today,
            SuccessMessage = TempData["DatLichSuccess"] as string
        });

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLich(DatLichKhamViewModel model)
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return redirect;
        }

        var benhNhan = await GetCurrentPatientAsync();
        if (benhNhan == null)
        {
            ModelState.AddModelError(string.Empty, "Không tìm thấy hồ sơ bệnh nhân cho tài khoản hiện tại.");
            return View(await BuildDatLichModelAsync(model));
        }

        if (!model.NgayKham.HasValue)
        {
            ModelState.AddModelError(nameof(model.NgayKham), "Vui lòng chọn ngày khám.");
        }

        if (!string.IsNullOrWhiteSpace(model.CaKham) && !ValidShifts.Contains(model.CaKham))
        {
            ModelState.AddModelError(nameof(model.CaKham), "Ca khám không hợp lệ.");
        }

        var ngayKham = model.NgayKham.HasValue ? DateOnly.FromDateTime(model.NgayKham.Value) : DateOnly.MinValue;
        if (model.NgayKham.HasValue && ngayKham < DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(nameof(model.NgayKham), "Ngày khám không được ở quá khứ.");
        }

        var gioKham = ParseAppointmentTime(model.GioKham);
        if (gioKham == null)
        {
            ModelState.AddModelError(nameof(model.GioKham), "Vui lòng chọn khung giờ khám hợp lệ.");
        }

        if (model.ThoiLuongKham is null or < 15 or > 180)
        {
            ModelState.AddModelError(nameof(model.ThoiLuongKham), "Thời lượng khám phải từ 15 đến 180 phút.");
        }

        if (gioKham.HasValue && model.ThoiLuongKham.HasValue && !string.IsNullOrWhiteSpace(model.CaKham))
        {
            if (!ShiftTimeRanges.TryGetValue(model.CaKham, out var shiftRange))
            {
                ModelState.AddModelError(nameof(model.CaKham), "Ca khám không hợp lệ.");
            }
            else if (!IsInShiftRange(gioKham.Value, model.ThoiLuongKham.Value, shiftRange))
            {
                ModelState.AddModelError(nameof(model.GioKham), "Khung giờ khám không nằm trong ca đã chọn.");
            }
        }

        var bacSi = await _context.BacSis
            .AsNoTracking()
            .Include(x => x.MaChuyenKhoaNavigation)
            .FirstOrDefaultAsync(x => x.MaBacSi == model.MaBacSi);

        if (bacSi == null)
        {
            ModelState.AddModelError(nameof(model.MaBacSi), "Bác sĩ đã chọn không tồn tại.");
        }
        else if (bacSi.MaChuyenKhoa != model.MaChuyenKhoa)
        {
            ModelState.AddModelError(nameof(model.MaBacSi), "Bác sĩ không thuộc chuyên khoa đã chọn.");
        }

        LichLamViec? lichLamViec = null;
        if (ModelState.IsValid && bacSi != null && model.CaKham != null)
        {
            var ngayTrongTuan = GetVietnameseDayOfWeek(model.NgayKham!.Value.DayOfWeek);
            lichLamViec = await _context.LichLamViecs
                .Include(x => x.MaPhongKhamNavigation)
                .FirstOrDefaultAsync(x =>
                    x.MaBacSi == bacSi.MaBacSi &&
                    x.NgayTrongTuan == ngayTrongTuan &&
                    x.CaLamViec == model.CaKham);

            if (lichLamViec?.MaPhongKhamNavigation == null)
            {
                ModelState.AddModelError(nameof(model.CaKham), "Bác sĩ không có lịch làm việc trong ngày và ca đã chọn.");
            }
            else if (lichLamViec.MaPhongKhamNavigation.TrangThai != "Hoạt động")
            {
                ModelState.AddModelError(nameof(model.CaKham), "Phòng khám của ca này hiện không hoạt động.");
            }
            else
            {
                var soLichDaDat = await _context.DangKyLichKhams.CountAsync(x =>
                    x.MaBacSi == bacSi.MaBacSi &&
                    x.MaPhongKham == lichLamViec.MaPhongKham &&
                    x.NgayKham == ngayKham &&
                    x.CaKham == model.CaKham &&
                    x.TrangThai != "Hủy");

                if (soLichDaDat >= lichLamViec.MaPhongKhamNavigation.SucChua)
                {
                    ModelState.AddModelError(nameof(model.CaKham), "Ca khám đã đầy. Vui lòng chọn thời gian khác.");
                }

                if (gioKham.HasValue && model.ThoiLuongKham.HasValue)
                {
                    var lichCungCa = await _context.DangKyLichKhams
                        .AsNoTracking()
                        .Where(x =>
                            x.MaBacSi == bacSi.MaBacSi &&
                            x.NgayKham == ngayKham &&
                            x.CaKham == model.CaKham &&
                            x.TrangThai != "Hủy" &&
                            x.GioKham != null &&
                            x.ThoiLuongKham != null)
                        .Select(x => new { x.GioKham, x.ThoiLuongKham })
                        .ToListAsync();

                    var biTrungGio = lichCungCa.Any(x =>
                        HasTimeOverlap(gioKham.Value, model.ThoiLuongKham.Value, x.GioKham!.Value, x.ThoiLuongKham!.Value));

                    if (biTrungGio)
                    {
                        ModelState.AddModelError(nameof(model.GioKham), "Khung giờ này đã có lịch khám khác. Vui lòng chọn giờ khác.");
                    }
                }
            }
        }

        if (!ModelState.IsValid || lichLamViec?.MaPhongKham == null)
        {
            return View(await BuildDatLichModelAsync(model));
        }

        var lichKham = new DangKyLichKham
        {
            MaDangKy = await GenerateAppointmentIdAsync(),
            MaBacSi = model.MaBacSi!,
            MaBenhNhan = benhNhan.MaBenhNhan,
            MaPhongKham = lichLamViec.MaPhongKham,
            NgayKham = ngayKham,
            CaKham = model.CaKham!,
            GioKham = gioKham,
            ThoiLuongKham = model.ThoiLuongKham,
            TrangThai = "Chờ khám"
        };

        _context.DangKyLichKhams.Add(lichKham);
        await _context.SaveChangesAsync();

        var gioKhamMessage = string.IsNullOrWhiteSpace(model.GioKham)
            ? string.Empty
            : $" Khung giờ dự kiến: {model.GioKham}, thời lượng khoảng {model.ThoiLuongKham ?? 30} phút.";
        TempData["DatLichSuccess"] = $"Đặt lịch khám thành công.{gioKhamMessage} Vui lòng theo dõi lịch hẹn và đến đúng thời gian đã chọn.";
        return RedirectToAction(nameof(DatLich));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GoiYChuyenKhoa([FromBody] SymptomSuggestionRequest request)
    {
        var redirect = RequirePatientRole();
        if (redirect != null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.TrieuChung))
        {
            return BadRequest(new { message = "Vui lòng nhập triệu chứng để hệ thống gợi ý chuyên khoa." });
        }

        var normalizedSymptoms = NormalizeText(request.TrieuChung);
        var specialties = await _context.ChuyenKhoas
            .AsNoTracking()
            .Select(x => new { x.MaChuyenKhoa, x.TenChuyenKhoa })
            .ToListAsync();

        var averageDurations = await _context.DichVuKhams
            .AsNoTracking()
            .Where(x => x.TrangThai == "Hoạt động")
            .GroupBy(x => x.MaChuyenKhoa)
            .Select(x => new
            {
                MaChuyenKhoa = x.Key,
                Minutes = (int)Math.Round(x.Average(service => service.ThoiGianTrungBinh ?? 30))
            })
            .ToDictionaryAsync(x => x.MaChuyenKhoa, x => x.Minutes);

        var suggestions = SymptomRules
            .Select(rule =>
            {
                var matchedKeywords = rule.Keywords
                    .Where(keyword => normalizedSymptoms.Contains(NormalizeText(keyword), StringComparison.Ordinal))
                    .ToList();

                var specialty = specialties.FirstOrDefault(item =>
                    NormalizeText(item.TenChuyenKhoa) == NormalizeText(rule.SpecialtyName));

                return new
                {
                    Rule = rule,
                    Specialty = specialty,
                    MatchedKeywords = matchedKeywords,
                    Score = matchedKeywords.Count
                };
            })
            .Where(item => item.Specialty != null && item.Score > 0)
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Rule.SpecialtyName)
            .Take(3)
            .Select(item => new SpecialtySuggestionResponse(
                item.Specialty!.MaChuyenKhoa,
                item.Specialty.TenChuyenKhoa,
                averageDurations.GetValueOrDefault(item.Specialty.MaChuyenKhoa, item.Rule.FallbackMinutes),
                $"Có liên quan đến: {string.Join(", ", item.MatchedKeywords.Take(4))}.",
                item.Score))
            .ToList();

        if (suggestions.Count == 0)
        {
            var generalSpecialty = specialties.FirstOrDefault(item => NormalizeText(item.TenChuyenKhoa) == NormalizeText("Nội khoa"))
                ?? specialties.FirstOrDefault();

            if (generalSpecialty != null)
            {
                suggestions.Add(new SpecialtySuggestionResponse(
                    generalSpecialty.MaChuyenKhoa,
                    generalSpecialty.TenChuyenKhoa,
                    averageDurations.GetValueOrDefault(generalSpecialty.MaChuyenKhoa, 30),
                    "Triệu chứng chưa đủ rõ để phân loại, nên bắt đầu từ chuyên khoa tổng quát.",
                    0));
            }
        }

        var urgentMatches = UrgentKeywords
            .Where(keyword => normalizedSymptoms.Contains(NormalizeText(keyword), StringComparison.Ordinal))
            .ToList();

        return Json(new SymptomSuggestionResult(
            suggestions,
            urgentMatches.Count > 0,
            urgentMatches.Count > 0
                ? $"Có dấu hiệu cần chú ý: {string.Join(", ", urgentMatches.Take(3))}. Nếu triệu chứng nặng hoặc diễn tiến nhanh, hãy đến cơ sở y tế gần nhất."
                : "Gợi ý chỉ mang tính hỗ trợ chọn chuyên khoa, không thay thế chẩn đoán của bác sĩ."));
    }

    private async Task<DatLichKhamViewModel> BuildDatLichModelAsync(DatLichKhamViewModel model)
    {
        var benhNhan = await GetCurrentPatientAsync();
        model.HoTenBenhNhan = benhNhan?.HoTen ?? HttpContext.Session.GetString("TenDangNhap") ?? "Bệnh nhân";
        model.MinNgayKham = DateTime.Today;

        model.ChuyenKhoas = await _context.ChuyenKhoas
            .AsNoTracking()
            .OrderBy(x => x.TenChuyenKhoa)
            .Select(x => new ChuyenKhoaOptionViewModel
            {
                MaChuyenKhoa = x.MaChuyenKhoa,
                TenChuyenKhoa = x.TenChuyenKhoa
            })
            .ToListAsync();

        model.BacSis = await _context.BacSis
            .AsNoTracking()
            .Include(x => x.MaChuyenKhoaNavigation)
            .OrderBy(x => x.HoTen)
            .Select(x => new BacSiOptionViewModel
            {
                MaBacSi = x.MaBacSi,
                HoTen = x.HoTen,
                TrinhDo = x.TrinhDo,
                MaChuyenKhoa = x.MaChuyenKhoa,
                TenChuyenKhoa = x.MaChuyenKhoaNavigation != null ? x.MaChuyenKhoaNavigation.TenChuyenKhoa : string.Empty
            })
            .ToListAsync();

        model.LichLamViecs = await _context.LichLamViecs
            .AsNoTracking()
            .Include(x => x.MaPhongKhamNavigation)
            .Where(x => x.MaPhongKhamNavigation != null && x.MaPhongKhamNavigation.TrangThai == "Hoạt động")
            .OrderBy(x => x.MaBacSi)
            .ThenBy(x => x.NgayTrongTuan)
            .ThenBy(x => x.CaLamViec)
            .Select(x => new LichLamViecOptionViewModel
            {
                MaBacSi = x.MaBacSi,
                NgayTrongTuan = x.NgayTrongTuan,
                CaLamViec = x.CaLamViec,
                MaPhongKham = x.MaPhongKham ?? string.Empty,
                TenPhongKham = x.MaPhongKhamNavigation != null ? x.MaPhongKhamNavigation.TenPhongKham : string.Empty,
                ViTri = x.MaPhongKhamNavigation != null ? x.MaPhongKhamNavigation.ViTri ?? string.Empty : string.Empty
            })
            .ToListAsync();

        return model;
    }

    private async Task<BenhNhan?> GetCurrentPatientAsync()
    {
        var maNguoiDung = HttpContext.Session.GetString("MaNguoiDung");
        if (string.IsNullOrWhiteSpace(maNguoiDung))
        {
            return null;
        }

        return await _context.BenhNhans.FirstOrDefaultAsync(x => x.MaNguoiDung == maNguoiDung);
    }

    private async Task<string> GenerateAppointmentIdAsync()
    {
        var ids = await _context.DangKyLichKhams
            .AsNoTracking()
            .Select(x => x.MaDangKy)
            .ToListAsync();

        var nextNumber = ids
            .Select(x => x.StartsWith("DK", StringComparison.OrdinalIgnoreCase) && int.TryParse(x[2..], out var number) ? number : 0)
            .DefaultIfEmpty()
            .Max() + 1;

        return $"DK{nextNumber:000}";
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

    private static string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ 2",
            DayOfWeek.Tuesday => "Thứ 3",
            DayOfWeek.Wednesday => "Thứ 4",
            DayOfWeek.Thursday => "Thứ 5",
            DayOfWeek.Friday => "Thứ 6",
            DayOfWeek.Saturday => "Thứ 7",
            _ => "Chủ nhật"
        };
    }

    private static TimeOnly? ParseAppointmentTime(string? value)
    {
        return TimeOnly.TryParseExact(value, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time)
            ? time
            : null;
    }

    private static bool IsInShiftRange(TimeOnly start, int durationMinutes, ShiftTimeRange range)
    {
        var end = start.AddMinutes(durationMinutes);
        return start >= range.Start && end <= range.End && end > start;
    }

    private static bool HasTimeOverlap(TimeOnly firstStart, int firstDuration, TimeOnly secondStart, int secondDuration)
    {
        var firstEnd = firstStart.AddMinutes(firstDuration);
        var secondEnd = secondStart.AddMinutes(secondDuration);

        return firstStart < secondEnd && secondStart < firstEnd;
    }

    private static string NormalizeText(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character == 'đ' ? 'd' : character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public sealed record SymptomSuggestionRequest(string? TrieuChung);

    private sealed record SymptomRule(string SpecialtyName, int FallbackMinutes, params string[] Keywords);

    private sealed record ShiftTimeRange(TimeOnly Start, TimeOnly End);

    private sealed record SpecialtySuggestionResponse(
        string SpecialtyId,
        string SpecialtyName,
        int EstimatedMinutes,
        string Reason,
        int Score);

    private sealed record SymptomSuggestionResult(
        List<SpecialtySuggestionResponse> Suggestions,
        bool HasUrgentWarning,
        string Message);
}
