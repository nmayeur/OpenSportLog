/* Copyright 2021 Nicolas Mayeur

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
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OSL.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                    table.UniqueConstraint("AK_Athletes_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ActivityEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AthleteId = table.Column<int>(nullable: true),
                    OriginId = table.Column<string>(nullable: true),
                    OriginSystem = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Calories = table.Column<int>(nullable: false),
                    Sport = table.Column<int>(nullable: false),
                    Time = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEntity_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ActivityEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackEntity_ActivityEntity_ActivityEntityId",
                        column: x => x.ActivityEntityId,
                        principalTable: "ActivityEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackSegmentEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackEntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackSegmentEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackSegmentEntity_TrackEntity_TrackEntityId",
                        column: x => x.TrackEntityId,
                        principalTable: "TrackEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackPointVO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTimeOffset>(nullable: false),
                    Latitude = table.Column<float>(nullable: false),
                    Longitude = table.Column<float>(nullable: false),
                    Elevation = table.Column<float>(nullable: false),
                    HeartRate = table.Column<int>(nullable: false),
                    Cadence = table.Column<int>(nullable: false),
                    TrackSegmentEntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPointVO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackPointVO_TrackSegmentEntity_TrackSegmentEntityId",
                        column: x => x.TrackSegmentEntityId,
                        principalTable: "TrackSegmentEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntity_AthleteId",
                table: "ActivityEntity",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackEntity_ActivityEntityId",
                table: "TrackEntity",
                column: "ActivityEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackPointVO_TrackSegmentEntityId",
                table: "TrackPointVO",
                column: "TrackSegmentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackSegmentEntity_TrackEntityId",
                table: "TrackSegmentEntity",
                column: "TrackEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackPointVO");

            migrationBuilder.DropTable(
                name: "TrackSegmentEntity");

            migrationBuilder.DropTable(
                name: "TrackEntity");

            migrationBuilder.DropTable(
                name: "ActivityEntity");

            migrationBuilder.DropTable(
                name: "Athletes");
        }
    }
}
