using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("NguoiDung")]
[Index("TenDangNhap", Name = "UQ__NguoiDun__55F68FC0286E3802", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaNguoiDung { get; set; } = null!;

    [StringLength(50)]
    public string? TenDangNhap { get; set; }

    [StringLength(100)]
    public string? MatKhau { get; set; }

    [StringLength(20)]
    public string? VaiTro { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual BacSi? BacSi { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual BenhNhan? BenhNhan { get; set; }
}
