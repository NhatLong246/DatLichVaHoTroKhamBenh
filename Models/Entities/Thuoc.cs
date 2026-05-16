using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("Thuoc")]
public partial class Thuoc
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaThuoc { get; set; } = null!;

    [StringLength(100)]
    public string TenThuoc { get; set; } = null!;

    [StringLength(20)]
    public string DonViTinh { get; set; } = null!;

    public int? SoLuongTon { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal GiaBan { get; set; }

    [InverseProperty("MaThuocNavigation")]
    public virtual ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();
}
