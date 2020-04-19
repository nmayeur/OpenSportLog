using GeoSports.Common.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSports.EF
{
    public class AthleteConfiguration : IEntityTypeConfiguration<AthleteEntity>
    {
        public void Configure(EntityTypeBuilder<AthleteEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.HasAlternateKey(x => x.Name); // Unique

            var activity = builder.OwnsMany<ActivityVO>(a => a.Activities, a =>
            {
                a.OwnsOne(x => x.Track).OwnsMany(x => x.TrackPoints, tp =>
                {
                    tp.Property<int>("Id");
                    tp.HasKey("Id");
                });
                a.HasKey("Id");
            });

        }
    }
}
