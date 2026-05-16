using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("DangKyLichKham")]
public partial class DangKyLichKham
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDangKy { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaBacSi { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaBenhNhan { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaPhongKham { get; set; } = null!;

    public DateOnly NgayKham { get; set; }

    [StringLength(10)]
    public string CaKham { get; set; } = null!;

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaBacSi")]
    [InverseProperty("DangKyLichKhams")]
    public virtual BacSi MaBacSiNavigation { get; set; } = null!;

    [ForeignKey("MaBenhNhan")]
    [InverseProperty("DangKyLichKhams")]
    public virtual BenhNhan MaBenhNhanNavigation { get; set; } = null!;

    [ForeignKey("MaPhongKham")]
    [InverseProperty("DangKyLichKhams")]
    public virtual PhongKham MaPhongKhamNavigation { get; set; } = null!;

    [InverseProperty("MaDangKyNavigation")]
    public virtual ICollection<PhieuKham> PhieuKhams { get; set; } = new List<PhieuKham>();
}
