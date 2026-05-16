using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("DonThuoc")]
public partial class DonThuoc
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDonThuoc { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    public DateOnly? NgayLap { get; set; }

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaDonThuocNavigation")]
    public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();

    [InverseProperty("MaDonThuocNavigation")]
    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    [ForeignKey("MaPhieuKham")]
    [InverseProperty("DonThuocs")]
    public virtual PhieuKham MaPhieuKhamNavigation { get; set; } = null!;
}
