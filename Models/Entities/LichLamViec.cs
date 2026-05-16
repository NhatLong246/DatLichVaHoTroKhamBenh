using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[PrimaryKey("MaBacSi", "NgayTrongTuan", "CaLamViec")]
[Table("LichLamViec")]
public partial class LichLamViec
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaBacSi { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string NgayTrongTuan { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string CaLamViec { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string? MaPhongKham { get; set; }

    [ForeignKey("MaBacSi")]
    [InverseProperty("LichLamViecs")]
    public virtual BacSi MaBacSiNavigation { get; set; } = null!;

    [ForeignKey("MaPhongKham")]
    [InverseProperty("LichLamViecs")]
    public virtual PhongKham? MaPhongKhamNavigation { get; set; }
}
