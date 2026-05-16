using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[PrimaryKey("MaPhieuKham", "MaDichVu")]
[Table("ChiTietDichVuKham")]
public partial class ChiTietDichVuKham
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDichVu { get; set; } = null!;

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(26, 2)")]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaDichVu")]
    [InverseProperty("ChiTietDichVuKhams")]
    public virtual DichVuKham MaDichVuNavigation { get; set; } = null!;

    [ForeignKey("MaPhieuKham")]
    [InverseProperty("ChiTietDichVuKhams")]
    public virtual PhieuKham MaPhieuKhamNavigation { get; set; } = null!;
}
