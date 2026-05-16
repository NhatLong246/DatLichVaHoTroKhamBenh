using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("ChuyenKhoa")]
public partial class ChuyenKhoa
{
    [Key]
    [StringLength(10)]
    [Unicode(false)]
    public string MaChuyenKhoa { get; set; } = null!;

    [StringLength(100)]
    public string TenChuyenKhoa { get; set; } = null!;

    [InverseProperty("MaChuyenKhoaNavigation")]
    public virtual ICollection<BacSi> BacSis { get; set; } = new List<BacSi>();

    [InverseProperty("MaChuyenKhoaNavigation")]
    public virtual ICollection<DichVuKham> DichVuKhams { get; set; } = new List<DichVuKham>();

    [InverseProperty("MaChuyenKhoaNavigation")]
    public virtual ICollection<PhongKham> PhongKhams { get; set; } = new List<PhongKham>();
}
