using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("DichVuKham")]
public partial class DichVuKham
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDichVu { get; set; } = null!;

    [StringLength(100)]
    public string TenDichVu { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string MaChuyenKhoa { get; set; } = null!;

    [Column(TypeName = "decimal(15, 2)")]
    public decimal GiaTien { get; set; }

    public int? ThoiGianTrungBinh { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaDichVuNavigation")]
    public virtual ICollection<ChiTietDichVuKham> ChiTietDichVuKhams { get; set; } = new List<ChiTietDichVuKham>();

    [ForeignKey("MaChuyenKhoa")]
    [InverseProperty("DichVuKhams")]
    public virtual ChuyenKhoa MaChuyenKhoaNavigation { get; set; } = null!;
}
