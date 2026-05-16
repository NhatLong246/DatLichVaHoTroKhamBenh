using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("ChiTietHoaDon")]
public partial class ChiTietHoaDon
{
    [Key]
    public int MaChiTiet { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string MaHoaDon { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string? MaDonThuoc { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal? TienKham { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal? TienThuoc { get; set; }

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaDonThuoc")]
    [InverseProperty("ChiTietHoaDons")]
    public virtual DonThuoc? MaDonThuocNavigation { get; set; }

    [ForeignKey("MaHoaDon")]
    [InverseProperty("ChiTietHoaDons")]
    public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;

    [ForeignKey("MaPhieuKham")]
    [InverseProperty("ChiTietHoaDons")]
    public virtual PhieuKham MaPhieuKhamNavigation { get; set; } = null!;
}
