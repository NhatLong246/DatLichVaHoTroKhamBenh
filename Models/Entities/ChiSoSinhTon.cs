using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("ChiSoSinhTon")]
public partial class ChiSoSinhTon
{
    [Key]
    public int MaChiSo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    public int? HuyetApTamThu { get; set; }

    public int? HuyetApTamTruong { get; set; }

    public int? NhipTim { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? ChieuCao { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? CanNang { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? BMI { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DuongHuyet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDo { get; set; }

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaPhieuKham")]
    [InverseProperty("ChiSoSinhTons")]
    public virtual PhieuKham MaPhieuKhamNavigation { get; set; } = null!;
}
