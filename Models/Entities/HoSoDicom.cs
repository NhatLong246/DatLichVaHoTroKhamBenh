using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.Entities;

[Table("HoSoDicom")]
public partial class HoSoDicom
{
    [Key]
    public int MaHoSo { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string MaPhieuKham { get; set; } = null!;

    [StringLength(255)]
    public string TenFile { get; set; } = null!;

    [StringLength(1000)]
    public string DuongDanFile { get; set; } = null!;

    [StringLength(50)]
    public string? LoaiHinhAnh { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTaiLen { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? KichThuoc { get; set; }

    [ForeignKey("MaPhieuKham")]
    [InverseProperty("HoSoDicoms")]
    public virtual PhieuKham MaPhieuKhamNavigation { get; set; } = null!;
}
