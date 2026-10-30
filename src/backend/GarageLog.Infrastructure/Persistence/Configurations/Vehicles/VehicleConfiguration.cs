using GarageLog.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GarageLog.Infrastructure.Persistence.Configurations.Vehicles;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OwnerId).IsRequired();

        builder.Property(x => x.Vin)
            .HasConversion(v => v.Value, s => Vin.From(s))
            .HasMaxLength(17)
            .IsRequired();

        builder.Property(x => x.LicensePlate)
            .HasConversion(l => l!.Value, s => LicensePlate.From(s))
            .HasMaxLength(12);

        builder.Property(x => x.Make).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Engine).HasMaxLength(64);
        builder.Property(x => x.Transmission).HasMaxLength(64);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.SoldAt);

        // История пробега — owned collection
        builder.OwnsMany(x => x.OdometerHistory, odo =>
        {
            odo.ToTable("odometer_readings");
            odo.WithOwner().HasForeignKey("vehicle_id");
            odo.Property<int>("id").ValueGeneratedOnAdd();
            odo.HasKey("id");

            odo.Property(x => x.Kilometers).HasColumnName("kilometers").IsRequired();
            odo.Property(x => x.TakenAt).HasColumnName("taken_at").IsRequired();
            odo.Property(x => x.Source).HasColumnName("source").HasConversion<int>().IsRequired();

            odo.HasIndex("vehicle_id", nameof(OdometerReading.TakenAt));
        });

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => new { x.OwnerId, x.Vin }).IsUnique();
        builder.HasIndex(x => x.OwnerId);
    }
}
