/* Copyright 2020 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSL.Common.Model;

namespace OSL.EF
{
    public class AthleteConfiguration : IEntityTypeConfiguration<AthleteEntity>
    {
        public void Configure(EntityTypeBuilder<AthleteEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.HasAlternateKey(x => x.Name); // Unique

            var activity = builder.OwnsMany<ActivityEntity>(a => a.Activities, a =>
            {
                a.OwnsMany(x => x.Tracks, track =>
                {
                    track.Property<int>("Id");
                    track.HasKey("Id");
                    track.OwnsMany(t => t.TrackSegments, segment =>
                    {
                        segment.Property<int>("Id");
                        segment.HasKey("Id");
                        segment.OwnsMany(s => s.TrackPoints, tp =>
                        {
                            tp.Property<int>("Id");
                            tp.HasKey("Id");
                        });
                    });
                });
                a.HasKey("Id");
            });

        }
    }
}
