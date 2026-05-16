using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("PhongKham")]
public partial class PhongKham
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPhongKham { get; set; } = null!;

    [StringLength(100)]
    public string TenPhongKham { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaChuyenKhoa { get; set; } = null!;

    [StringLength(200)]
    public string? ViTri { get; set; }

    public int SucChua { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    [InverseProperty("MaPhongKhamNavigation")]
    public virtual ICollection<DangKyLichKham> DangKyLichKhams { get; set; } = new List<DangKyLichKham>();

    [InverseProperty("MaPhongKhamNavigation")]
    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    [ForeignKey("MaChuyenKhoa")]
    [InverseProperty("PhongKhams")]
    public virtual ChuyenKhoa MaChuyenKhoaNavigation { get; set; } = null!;
}
