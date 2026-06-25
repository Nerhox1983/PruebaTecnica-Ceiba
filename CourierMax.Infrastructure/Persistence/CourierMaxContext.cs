using CourierMax.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CourierMax.Infrastructure.Persistence
{
    public class CourierMaxContext : DbContext
    {
        public CourierMaxContext(DbContextOptions<CourierMaxContext> options) : base(options) { }

        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
        public DbSet<CityDistance> CityDistances => Set<CityDistance>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CityDistance>(entity =>
            {
                entity.ToTable("CityDistances", "dbo");

                entity.HasKey(e => new { e.OriginCityId, e.DestinationCityId });

                entity.Property(e => e.OriginCityId).HasColumnName("OriginCityId").IsRequired();
                entity.Property(e => e.DestinationCityId).HasColumnName("DestinationCityId").IsRequired();

                entity.Property(e => e.DistanceKm)
                    .HasColumnName("DistanceKm")                    
                    .IsRequired();

                entity.Property(e => e.DistanceTariff)
                    .HasColumnName("DistanceTariff")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });
            
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("Shipments", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("ShipmentId");

                entity.Property(e => e.SenderName).HasColumnName("SenderName").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ReceiverName).HasColumnName("ReceiverName").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ReceiverAddress).HasColumnName("ReceiverAddress").HasMaxLength(250).IsRequired();
                entity.Property(e => e.DestinationCityId).HasColumnName("DestinationCityId").IsRequired();
                entity.Property(e => e.WeightKg).HasColumnName("WeightKg").IsRequired();
                entity.Property(e => e.HeightCm).HasColumnName("HeightCm");
                entity.Property(e => e.TrackingCode).HasColumnName("TrackingCode").HasMaxLength(50).IsRequired();

                entity.Property(e => e.CurrentStatus)
                    .HasColumnName("CurrentStatus")
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.PackageType)
                    .HasColumnName("PackageType")
                    .HasConversion(
                        v => v.ToString(),
                        v => ConvertToPackageType(v)
                    );

                entity.Property(e => e.ServiceType)
                    .HasColumnName("ServiceType")
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.TotalCost).HasColumnName("TotalCost").HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.EstimatedDeliveryDate).HasColumnName("EstimatedDeliveryDate").IsRequired();

                entity.Property(e => e.VehicleId)
                    .HasColumnName("VehicleId")
                    .IsRequired(false);
                
                entity.Property(e => e.BaseTariff).HasColumnName("BaseTariff").HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.WeightSurcharge).HasColumnName("WeightSurcharge").HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.DistanceSurcharge).HasColumnName("DistanceSurcharge").HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.PackageTypeSurcharge).HasColumnName("PackageTypeSurcharge").HasColumnType("decimal(18,2)").IsRequired();

                var navigation = entity.Metadata.FindNavigation(nameof(Shipment.StatusHistory));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<ShipmentStatusHistory>(entity =>
            {
                entity.ToTable("ShipmentStatusLogs", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("StatusLogId");

                entity.Property(e => e.ShipmentId).HasColumnName("ShipmentId").IsRequired();

                entity.Property(e => e.PreviousStatus).HasColumnName("PreviousStatus");
                entity.Property(e => e.NewStatus).HasColumnName("NewStatus").IsRequired();

                entity.Property(e => e.ChangedAt).HasColumnName("ChangedAt").IsRequired();
                entity.Property(e => e.ChangeReason).HasColumnName("ChangeReason");
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();

                entity.HasOne<Shipment>()
                    .WithMany(s => s.StatusHistory)
                    .HasForeignKey(e => e.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static CourierMax.Domain.Enums.PackageType ConvertToPackageType(string value)
        {
            return Enum.TryParse<CourierMax.Domain.Enums.PackageType>(value, out var result)
                ? result
                : CourierMax.Domain.Enums.PackageType.Paquete;
        }
    }
}