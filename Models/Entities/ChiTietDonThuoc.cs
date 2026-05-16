using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[PrimaryKey("MaDonThuoc", "MaThuoc")]
[Table("ChiTietDonThuoc")]
public partial class ChiTietDonThuoc
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDonThuoc { get; set; } = null!;

    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaThuoc { get; set; } = null!;

    public int SoLuong { get; set; }

    [StringLength(50)]
    public string LieuLuong { get; set; } = null!;

    public int SoNgayDung { get; set; }

    [StringLength(1000)]
    public string CachDung { get; set; } = null!;

    [Column(TypeName = "decimal(15, 2)")]
    public decimal DonGia { get; set; }

    [Column(TypeName = "decimal(26, 2)")]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaDonThuoc")]
    [InverseProperty("ChiTietDonThuocs")]
    public virtual DonThuoc MaDonThuocNavigation { get; set; } = null!;

    [ForeignKey("MaThuoc")]
    [InverseProperty("ChiTietDonThuocs")]
    public virtual Thuoc MaThuocNavigation { get; set; } = null!;
}
