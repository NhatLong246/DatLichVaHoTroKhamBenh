using System;
using System.Collections.Generic;
using HeThongDatLichVaKhamBenh.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HeThongDatLichVaKhamBenh.Models.EF;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BacSi> BacSis { get; set; }

    public virtual DbSet<BenhNhan> BenhNhans { get; set; }

    public virtual DbSet<ChiTietDichVuKham> ChiTietDichVuKhams { get; set; }

    public virtual DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<ChuyenKhoa> ChuyenKhoas { get; set; }

    public virtual DbSet<DangKyLichKham> DangKyLichKhams { get; set; }

    public virtual DbSet<DichVuKham> DichVuKhams { get; set; }

    public virtual DbSet<DonThuoc> DonThuocs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LichLamViec> LichLamViecs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhieuKham> PhieuKhams { get; set; }

    public virtual DbSet<PhongKham> PhongKhams { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacSi>(entity =>
        {
            entity.HasKey(e => e.MaBacSi).HasName("PK__BacSi__E022715E72571D70");

            entity.HasOne(d => d.MaChuyenKhoaNavigation).WithMany(p => p.BacSis).HasConstraintName("FK__BacSi__MaChuyenK__6FE99F9F");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithOne(p => p.BacSi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BacSi__MaNguoiDu__6EF57B66");
        });

        modelBuilder.Entity<BenhNhan>(entity =>
        {
            entity.HasKey(e => e.MaBenhNhan).HasName("PK__BenhNhan__22A8B330AC3D7885");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithOne(p => p.BenhNhan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BenhNhan__MaNguo__68487DD7");
        });

        modelBuilder.Entity<ChiTietDichVuKham>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuKham, e.MaDichVu }).HasName("PK__ChiTietD__16C438377DB5E9D3");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.ChiTietDichVuKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDi__MaDic__0B91BA14");

            entity.HasOne(d => d.MaPhieuKhamNavigation).WithMany(p => p.ChiTietDichVuKhams).HasConstraintName("FK__ChiTietDi__MaPhi__0A9D95DB");
        });

        modelBuilder.Entity<ChiTietDonThuoc>(entity =>
        {
            entity.HasKey(e => new { e.MaDonThuoc, e.MaThuoc }).HasName("PK__ChiTietD__2A42818367DEE93D");

            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaDonThuocNavigation).WithMany(p => p.ChiTietDonThuocs).HasConstraintName("FK__ChiTietDo__MaDon__17F790F9");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.ChiTietDonThuocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietDo__MaThu__18EBB532");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietH__CDF0A114B44AE665");

            entity.Property(e => e.TienKham).HasDefaultValue(0m);
            entity.Property(e => e.TienThuoc).HasDefaultValue(0m);

            entity.HasOne(d => d.MaDonThuocNavigation).WithMany(p => p.ChiTietHoaDons).HasConstraintName("FK__ChiTietHo__MaDon__2B0A656D");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons).HasConstraintName("FK__ChiTietHo__MaHoa__29221CFB");

            entity.HasOne(d => d.MaPhieuKhamNavigation).WithMany(p => p.ChiTietHoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHo__MaPhi__2A164134");
        });

        modelBuilder.Entity<ChuyenKhoa>(entity =>
        {
            entity.HasKey(e => e.MaChuyenKhoa).HasName("PK__ChuyenKh__CD0E428F07CF1CA5");
        });

        modelBuilder.Entity<DangKyLichKham>(entity =>
        {
            entity.HasKey(e => e.MaDangKy).HasName("PK__DangKyLi__BA90F02D1D43BD61");

            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ khám");

            entity.HasOne(d => d.MaBacSiNavigation).WithMany(p => p.DangKyLichKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyLic__MaBac__7D439ABD");

            entity.HasOne(d => d.MaBenhNhanNavigation).WithMany(p => p.DangKyLichKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyLic__MaBen__7E37BEF6");

            entity.HasOne(d => d.MaPhongKhamNavigation).WithMany(p => p.DangKyLichKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DangKyLic__MaPho__7F2BE32F");
        });

        modelBuilder.Entity<DichVuKham>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVuKh__C0E6DE8FB0B58D2A");

            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.MaChuyenKhoaNavigation).WithMany(p => p.DichVuKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DichVuKha__MaChu__5812160E");
        });

        modelBuilder.Entity<DonThuoc>(entity =>
        {
            entity.HasKey(e => e.MaDonThuoc).HasName("PK__DonThuoc__3EF99EE1F6DA2135");

            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ cấp thuốc");

            entity.HasOne(d => d.MaPhieuKhamNavigation).WithMany(p => p.DonThuocs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonThuoc__MaPhie__14270015");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B8AEF8562");

            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0m);
            entity.Property(e => e.TrangThai).HasDefaultValue("Chưa thanh toán");

            entity.HasOne(d => d.MaBenhNhanNavigation).WithMany(p => p.HoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaBenhNh__2180FB33");
        });

        modelBuilder.Entity<LichLamViec>(entity =>
        {
            entity.HasKey(e => new { e.MaBacSi, e.NgayTrongTuan, e.CaLamViec }).HasName("PK__LichLamV__5EE9A4F9A4D846A2");

            entity.HasOne(d => d.MaBacSiNavigation).WithMany(p => p.LichLamViecs).HasConstraintName("FK__LichLamVi__MaBac__778AC167");

            entity.HasOne(d => d.MaPhongKhamNavigation).WithMany(p => p.LichLamViecs).HasConstraintName("FK__LichLamVi__MaPho__76969D2E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762C95B1020");

            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<PhieuKham>(entity =>
        {
            entity.HasKey(e => e.MaPhieuKham).HasName("PK__PhieuKha__FACA55DFED5C57DF");

            entity.Property(e => e.TrangThai).HasDefaultValue("Đang xử lý");

            entity.HasOne(d => d.MaDangKyNavigation).WithMany(p => p.PhieuKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuKham__MaDan__05D8E0BE");
        });

        modelBuilder.Entity<PhongKham>(entity =>
        {
            entity.HasKey(e => e.MaPhongKham).HasName("PK__PhongKha__D31EFFBA2789F21D");

            entity.Property(e => e.TrangThai).HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.MaChuyenKhoaNavigation).WithMany(p => p.PhongKhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhongKham__MaChu__52593CB8");
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.MaThuoc).HasName("PK__Thuoc__4BB1F620AC73EA25");

            entity.Property(e => e.SoLuongTon).HasDefaultValue(0);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
