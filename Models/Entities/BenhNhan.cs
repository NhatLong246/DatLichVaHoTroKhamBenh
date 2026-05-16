using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("BenhNhan")]
[Index("MaNguoiDung", Name = "UQ__BenhNhan__C539D763D7E774FB", IsUnique = true)]
public partial class BenhNhan
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaBenhNhan { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(200)]
    public string? DiaChi { get; set; }

    [InverseProperty("MaBenhNhanNavigation")]
    public virtual ICollection<DangKyLichKham> DangKyLichKhams { get; set; } = new List<DangKyLichKham>();

    [InverseProperty("MaBenhNhanNavigation")]
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("BenhNhan")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
