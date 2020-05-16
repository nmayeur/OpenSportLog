﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OSL.EF;

namespace OSL.EF.Migrations
{
    [DbContext(typeof(OpenSportLogContext))]
    [Migration("20200516085557_Fixes")]
    partial class Fixes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("OSL.Common.Model.AthleteEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Athletes");
                });

            modelBuilder.Entity("OSL.Common.Model.AthleteEntity", b =>
                {
                    b.OwnsMany("OSL.Common.Model.ActivityEntity", "Activities", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int>("AthleteEntityId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Calories")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Location")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Name")
                                .HasColumnType("TEXT");

                            b1.Property<string>("OriginId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("OriginSystem")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Sport")
                                .HasColumnType("INTEGER");

                            b1.Property<DateTimeOffset>("Time")
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("AthleteEntityId");

                            b1.ToTable("ActivityEntity");

                            b1.WithOwner()
                                .HasForeignKey("AthleteEntityId");

                            b1.OwnsMany("OSL.Common.Model.TrackEntity", "Tracks", b2 =>
                                {
                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("INTEGER");

                                    b2.Property<int>("ActivityEntityId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("Name")
                                        .HasColumnType("TEXT");

                                    b2.HasKey("Id");

                                    b2.HasIndex("ActivityEntityId");

                                    b2.ToTable("TrackEntity");

                                    b2.WithOwner()
                                        .HasForeignKey("ActivityEntityId");

                                    b2.OwnsMany("OSL.Common.Model.TrackSegmentEntity", "TrackSegments", b3 =>
                                        {
                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("INTEGER");

                                            b3.Property<int>("TrackEntityId")
                                                .HasColumnType("INTEGER");

                                            b3.HasKey("Id");

                                            b3.HasIndex("TrackEntityId");

                                            b3.ToTable("TrackSegmentEntity");

                                            b3.WithOwner()
                                                .HasForeignKey("TrackEntityId");

                                            b3.OwnsMany("OSL.Common.Model.TrackPointVO", "TrackPoints", b4 =>
                                                {
                                                    b4.Property<int>("Id")
                                                        .ValueGeneratedOnAdd()
                                                        .HasColumnType("INTEGER");

                                                    b4.Property<int>("Cadence")
                                                        .HasColumnType("INTEGER");

                                                    b4.Property<float>("Elevation")
                                                        .HasColumnType("REAL");

                                                    b4.Property<int>("HeartRate")
                                                        .HasColumnType("INTEGER");

                                                    b4.Property<float>("Latitude")
                                                        .HasColumnType("REAL");

                                                    b4.Property<float>("Longitude")
                                                        .HasColumnType("REAL");

                                                    b4.Property<DateTimeOffset>("Time")
                                                        .HasColumnType("TEXT");

                                                    b4.Property<int>("TrackSegmentEntityId")
                                                        .HasColumnType("INTEGER");

                                                    b4.HasKey("Id");

                                                    b4.HasIndex("TrackSegmentEntityId");

                                                    b4.ToTable("TrackPointVO");

                                                    b4.WithOwner()
                                                        .HasForeignKey("TrackSegmentEntityId");
                                                });
                                        });
                                });
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
