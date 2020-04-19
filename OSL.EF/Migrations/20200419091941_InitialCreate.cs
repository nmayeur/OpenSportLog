using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeoSports.EF.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                    table.UniqueConstraint("AK_Athletes_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ActivityVO",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Calories = table.Column<int>(nullable: false),
                    Sport = table.Column<int>(nullable: false),
                    AthleteEntityId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityVO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityVO_Athletes_AthleteEntityId",
                        column: x => x.AthleteEntityId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    HearRate = table.Column<int>(nullable: false),
                    Cadence = table.Column<int>(nullable: false),
                    TrackVOActivityVOId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPointVO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackPointVO_ActivityVO_TrackVOActivityVOId",
                        column: x => x.TrackVOActivityVOId,
                        principalTable: "ActivityVO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityVO_AthleteEntityId",
                table: "ActivityVO",
                column: "AthleteEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackPointVO_TrackVOActivityVOId",
                table: "TrackPointVO",
                column: "TrackVOActivityVOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackPointVO");

            migrationBuilder.DropTable(
                name: "ActivityVO");

            migrationBuilder.DropTable(
                name: "Athletes");
        }
    }
}
