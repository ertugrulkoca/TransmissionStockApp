using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Models.Entities;

namespace TransmissionStockApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TransmissionStock> TransmissionStocks { get; set; }
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<TransmissionStatus> TransmissionStatuses { get; set; }
        public DbSet<TransmissionDriveType> TransmissionDriveTypes { get; set; }
        public DbSet<TransmissionBrand> TransmissionBrands { get; set; }

        // ✅ isim düzeltildi
        public DbSet<Shelf> Shelves { get; set; }

        public DbSet<TransmissionStockLocation> TransmissionStockLocations { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("Turkish_CI_AS");

            // -------------------------
            // VehicleBrand -> VehicleModel (Cascade)
            // -------------------------
            modelBuilder.Entity<VehicleBrand>()
                .HasMany(b => b.Models)
                .WithOne(m => m.VehicleBrand)
                .HasForeignKey(m => m.VehicleBrandId)
                .OnDelete(DeleteBehavior.Cascade);

            // -------------------------
            // VehicleModel -> TransmissionStock (SetNull)
            // -------------------------
            modelBuilder.Entity<TransmissionStock>()
                .HasOne(ts => ts.VehicleModel)
                .WithMany(vm => vm.TransmissionStocks)
                .HasForeignKey(ts => ts.VehicleModelId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // -------------------------
            // VehicleBrand -> TransmissionStock (Restrict)
            // -------------------------
            modelBuilder.Entity<TransmissionStock>()
                .HasOne(ts => ts.VehicleBrand)
                .WithMany(vb => vb.TransmissionStocks)
                .HasForeignKey(ts => ts.VehicleBrandId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // TransmissionStatus -> TransmissionStock (Restrict)
            // -------------------------
            modelBuilder.Entity<TransmissionStatus>()
                .HasMany(s => s.TransmissionStocks)
                .WithOne(ts => ts.TransmissionStatus)
                .HasForeignKey(ts => ts.TransmissionStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // TransmissionDriveType -> TransmissionStock (SetNull)
            // -------------------------
            modelBuilder.Entity<TransmissionDriveType>()
                .HasMany(dt => dt.TransmissionStocks)
                .WithOne(ts => ts.TransmissionDriveType)
                .HasForeignKey(ts => ts.TransmissionDriveTypeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // -------------------------
            // TransmissionBrand -> TransmissionStock (Restrict)
            // -------------------------
            modelBuilder.Entity<TransmissionBrand>()
                .HasMany(tb => tb.TransmissionStocks)
                .WithOne(ts => ts.TransmissionBrand)
                .HasForeignKey(ts => ts.TransmissionBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // Warehouse -> Shelf (Restrict)
            // -------------------------
            modelBuilder.Entity<Warehouse>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique();

                e.HasMany(w => w.Shelves)
                 .WithOne(s => s.Warehouse)
                 .HasForeignKey(s => s.WarehouseId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------------
            // Shelf config (max length + depot içi unique)
            // -------------------------
            modelBuilder.Entity<Shelf>(e =>
            {
                e.Property(x => x.ShelfCode).IsRequired().HasMaxLength(50);

                e.HasIndex(x => new { x.WarehouseId, x.ShelfCode })
                 .IsUnique();
            });

            // -------------------------
            // M2M Join: TransmissionStockLocation
            // -------------------------
            modelBuilder.Entity<TransmissionStockLocation>(e =>
            {
                e.HasKey(x => new { x.TransmissionStockId, x.ShelfId });

                e.Property(x => x.Quantity).IsRequired();

                e.ToTable(tb => tb.HasCheckConstraint("CK_TSL_Quantity_NonNegative", "[Quantity] >= 0"));

                e.HasOne(x => x.TransmissionStock)
                 .WithMany(ts => ts.TransmissionStockLocations)
                 .HasForeignKey(x => x.TransmissionStockId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Shelf)
                 .WithMany(s => s.TransmissionStockLocations)
                 .HasForeignKey(x => x.ShelfId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => x.ShelfId);
            });

            // -------------------------
            // Unique indexler (diğerleri)
            // -------------------------
            modelBuilder.Entity<VehicleBrand>()
                .HasIndex(vb => vb.Name).IsUnique();

            modelBuilder.Entity<VehicleModel>()
                .HasIndex(vm => new { vm.VehicleBrandId, vm.Name }).IsUnique();

            modelBuilder.Entity<TransmissionStatus>()
                .HasIndex(ts => ts.Name).IsUnique();

            modelBuilder.Entity<TransmissionBrand>()
                .HasIndex(tb => tb.Name).IsUnique();

            modelBuilder.Entity<TransmissionDriveType>()
                .HasIndex(dt => dt.Name).IsUnique();

            modelBuilder.Entity<TransmissionStock>()
                .HasIndex(ts => new { ts.TransmissionBrandId, ts.SparePartNo, ts.TransmissionStatusId })
                .IsUnique();
        }
    }



}
