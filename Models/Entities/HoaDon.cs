using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("HoaDon")]
public partial class HoaDon
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaHoaDon { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaBenhNhan { get; set; } = null!;

    public DateOnly? NgayLap { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal? TongTien { get; set; }

    [StringLength(50)]
    public string? HinhThucThanhToan { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaHoaDonNavigation")]
    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    [ForeignKey("MaBenhNhan")]
    [InverseProperty("HoaDons")]
    public virtual BenhNhan MaBenhNhanNavigation { get; set; } = null!;
}
