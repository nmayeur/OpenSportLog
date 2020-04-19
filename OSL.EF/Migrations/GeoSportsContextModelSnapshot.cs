﻿/* Copyright 2020 Nicolas Mayeur

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

// <auto-generated />
using System;
using GeoSports.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeoSports.EF.Migrations
{
    [DbContext(typeof(GeoSportsContext))]
    partial class GeoSportsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("GeoSports.Common.model.AthleteEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Athletes");
                });

            modelBuilder.Entity("GeoSports.Common.model.AthleteEntity", b =>
                {
                    b.OwnsMany("GeoSports.Common.model.ActivityVO", "Activities", b1 =>
                        {
                            b1.Property<string>("Id")
                                .HasColumnType("TEXT");

                            b1.Property<string>("AthleteEntityId")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("Calories")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Location")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Name")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Sport")
                                .HasColumnType("INTEGER");

                            b1.HasKey("Id");

                            b1.HasIndex("AthleteEntityId");

                            b1.ToTable("ActivityVO");

                            b1.WithOwner()
                                .HasForeignKey("AthleteEntityId");

                            b1.OwnsOne("GeoSports.Common.model.TrackVO", "Track", b2 =>
                                {
                                    b2.Property<string>("ActivityVOId")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("ActivityVOId");

                                    b2.ToTable("ActivityVO");

                                    b2.WithOwner()
                                        .HasForeignKey("ActivityVOId");

                                    b2.OwnsMany("GeoSports.Common.model.TrackPointVO", "TrackPoints", b3 =>
                                        {
                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("INTEGER");

                                            b3.Property<int>("Cadence")
                                                .HasColumnType("INTEGER");

                                            b3.Property<float>("Elevation")
                                                .HasColumnType("REAL");

                                            b3.Property<int>("HearRate")
                                                .HasColumnType("INTEGER");

                                            b3.Property<float>("Latitude")
                                                .HasColumnType("REAL");

                                            b3.Property<float>("Longitude")
                                                .HasColumnType("REAL");

                                            b3.Property<DateTimeOffset>("Time")
                                                .HasColumnType("TEXT");

                                            b3.Property<string>("TrackVOActivityVOId")
                                                .IsRequired()
                                                .HasColumnType("TEXT");

                                            b3.HasKey("Id");

                                            b3.HasIndex("TrackVOActivityVOId");

                                            b3.ToTable("TrackPointVO");

                                            b3.WithOwner()
                                                .HasForeignKey("TrackVOActivityVOId");
                                        });
                                });
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
