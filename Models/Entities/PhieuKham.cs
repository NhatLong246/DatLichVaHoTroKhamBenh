using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("PhieuKham")]
public partial class PhieuKham
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaDangKy { get; set; } = null!;

    [StringLength(1000)]
    public string? TrieuChung { get; set; }

    [StringLength(1000)]
    public string? ChanDoan { get; set; }

    [StringLength(1000)]
    public string? HuongDieuTri { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaPhieuKhamNavigation")]
    public virtual ICollection<ChiTietDichVuKham> ChiTietDichVuKhams { get; set; } = new List<ChiTietDichVuKham>();

    [InverseProperty("MaPhieuKhamNavigation")]
    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    [InverseProperty("MaPhieuKhamNavigation")]
    public virtual ICollection<DonThuoc> DonThuocs { get; set; } = new List<DonThuoc>();

    [ForeignKey("MaDangKy")]
    [InverseProperty("PhieuKhams")]
    public virtual DangKyLichKham MaDangKyNavigation { get; set; } = null!;
}
