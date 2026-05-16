using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("BacSi")]
[Index("MaNguoiDung", Name = "UQ__BacSi__C539D763F5172AC8", IsUnique = true)]
public partial class BacSi
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaBacSi { get; set; } = null!;

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

    [StringLength(100)]
    public string? TrinhDo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? MaChuyenKhoa { get; set; }

    [InverseProperty("MaBacSiNavigation")]
    public virtual ICollection<DangKyLichKham> DangKyLichKhams { get; set; } = new List<DangKyLichKham>();

    [InverseProperty("MaBacSiNavigation")]
    public virtual ICollection<LichLamViec> LichLamViecs { get; set; } = new List<LichLamViec>();

    [ForeignKey("MaChuyenKhoa")]
    [InverseProperty("BacSis")]
    public virtual ChuyenKhoa? MaChuyenKhoaNavigation { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("BacSi")]
    public virtual NguoiDung MaNguoiDungNavigation { get; set; } = null!;
}
