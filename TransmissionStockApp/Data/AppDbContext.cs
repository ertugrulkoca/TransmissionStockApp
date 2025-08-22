using Microsoft.EntityFrameworkCore;
using TransmissionStockApp.Models.Entities;

namespace TransmissionStockApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet'ler
        public DbSet<TransmissionStock> TransmissionStocks { get; set; }
        public DbSet<VehicleBrand> VehicleBrands { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }
        public DbSet<TransmissionStatus> TransmissionStatuses { get; set; }
        public DbSet<TransmissionDriveType> TransmissionDriveTypes { get; set; }
        public DbSet<StockLocation> StockLocations { get; set; }
        public DbSet<TransmissionBrand> TransmissionBrands { get; set; }
        public DbSet<TransmissionStockLocation> TransmissionStockLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("Turkish_CI_AS");

            // VehicleBrand -> VehicleModel (Cascade)
            modelBuilder.Entity<VehicleBrand>()
                .HasMany(b => b.Models)
                .WithOne(m => m.VehicleBrand)
                .HasForeignKey(m => m.VehicleBrandId)
                .OnDelete(DeleteBehavior.Cascade);

            // VehicleModel -> TransmissionStock (SetNull)
            modelBuilder.Entity<TransmissionStock>()
                .HasOne(ts => ts.VehicleModel)
                .WithMany(vm => vm.TransmissionStocks)
                .HasForeignKey(ts => ts.VehicleModelId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // VehicleBrand -> TransmissionStock (Restrict)  // multiple cascade path'i engeller
            modelBuilder.Entity<TransmissionStock>()
                .HasOne(ts => ts.VehicleBrand)
                .WithMany(vb => vb.TransmissionStocks) // VehicleBrand'da koleksiyon varsa; yoksa .WithMany()
                .HasForeignKey(ts => ts.VehicleBrandId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // TransmissionStatus -> TransmissionStock (Restrict)
            modelBuilder.Entity<TransmissionStatus>()
                .HasMany(s => s.TransmissionStocks)
                .WithOne(ts => ts.TransmissionStatus)
                .HasForeignKey(ts => ts.TransmissionStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransmissionDriveType -> TransmissionStock (SetNull)
            modelBuilder.Entity<TransmissionDriveType>()
                .HasMany(dt => dt.TransmissionStocks)
                .WithOne(ts => ts.TransmissionDriveType)
                .HasForeignKey(ts => ts.TransmissionDriveTypeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // TransmissionBrand -> TransmissionStock (Restrict)
            modelBuilder.Entity<TransmissionBrand>()
                .HasMany(tb => tb.TransmissionStocks)
                .WithOne(ts => ts.TransmissionBrand)
                .HasForeignKey(ts => ts.TransmissionBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // M2M: TransmissionStock <-> StockLocation
            modelBuilder.Entity<TransmissionStockLocation>()
                .HasKey(tsl => new { tsl.TransmissionStockId, tsl.StockLocationId });

            // Non-negative quantity check
            modelBuilder.Entity<TransmissionStockLocation>()
                .ToTable(tb => tb.HasCheckConstraint("CK_TSL_Quantity_NonNegative", "[Quantity] >= 0"));

            // TSL: Stock silinirse bağlar silinsin; Lokasyon silinmesin (Restrict)
            modelBuilder.Entity<TransmissionStockLocation>()
                .HasOne(tsl => tsl.TransmissionStock)
                .WithMany(ts => ts.TransmissionStockLocations)
                .HasForeignKey(tsl => tsl.TransmissionStockId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransmissionStockLocation>()
                .HasOne(tsl => tsl.StockLocation)
                .WithMany(sl => sl.TransmissionStockLocations)
                .HasForeignKey(tsl => tsl.StockLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique indexler
            modelBuilder.Entity<VehicleBrand>()
                .HasIndex(vb => vb.Name)
                .IsUnique();

            modelBuilder.Entity<VehicleModel>()
                .HasIndex(vm => new { vm.VehicleBrandId, vm.Name })
                .IsUnique();

            modelBuilder.Entity<TransmissionStatus>()
                .HasIndex(ts => ts.Name)
                .IsUnique();

            modelBuilder.Entity<TransmissionBrand>()
                .HasIndex(tb => tb.Name)
                .IsUnique();

            modelBuilder.Entity<StockLocation>()
                .HasIndex(sl => sl.ShelfCode)
                .IsUnique();

            // TransmissionStock tekilliği: (TransmissionBrandId + SparePartNo + TransmissionStatusId)
            modelBuilder.Entity<TransmissionStock>()
                .HasIndex(ts => new { ts.TransmissionBrandId, ts.SparePartNo, ts.TransmissionStatusId })
                .IsUnique();

            modelBuilder.Entity<TransmissionDriveType>()
                .HasIndex(dt => dt.Name)
                .IsUnique(); // veya Value için

        }

    }

}
