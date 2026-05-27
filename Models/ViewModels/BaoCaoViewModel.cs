using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeThongDatLichVaKhamBenh.Models.ViewModels;

public class BaoCaoFilterViewModel
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public string? MaChuyenKhoa { get; set; }
    public string? MaBacSi { get; set; }
}

public class BaoCaoHoaDonItem
{
    public string MaHoaDon { get; set; } = null!;
    public string TenBenhNhan { get; set; } = null!;
    public DateOnly NgayLap { get; set; }
    public decimal TongTien { get; set; }
}

public class BaoCaoChuyenKhoaChartItem
{
    public string TenChuyenKhoa { get; set; } = null!;
    public int SoLuotKham { get; set; }
    public string Color { get; set; } = null!;
}

public class BaoCaoBacSiItem
{
    public string MaBacSi { get; set; } = null!;
    public string TenBacSi { get; set; } = null!;
    public string TenChuyenKhoa { get; set; } = null!;
    public int SoLuotKham { get; set; }
    public decimal DoanhThuMangLai { get; set; }
}

public class BaoCaoViewModel
{
    public BaoCaoFilterViewModel Filter { get; set; } = new();

    public decimal TongDoanhThu { get; set; }
    public int TongLuotKham { get; set; }
    public int TongHoaDon { get; set; }

    public List<BaoCaoHoaDonItem> DanhSachHoaDon { get; set; } = new();
    public List<BaoCaoChuyenKhoaChartItem> ChartChuyenKhoa { get; set; } = new();
    public List<BaoCaoBacSiItem> ThongKeBacSi { get; set; } = new();

    public List<SelectListItem> DanhSachChuyenKhoa { get; set; } = new();
    public List<SelectListItem> DanhSachBacSi { get; set; } = new();
}
