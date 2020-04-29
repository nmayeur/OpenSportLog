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
                    Id = table.Column<string>(nullable: false),
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
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Calories = table.Column<int>(nullable: false),
                    Sport = table.Column<int>(nullable: false),
                    AthleteEntityId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityEntity_Athletes_AthleteEntityId",
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
                    TrackEntityActivityEntityId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPointVO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackPointVO_ActivityEntity_TrackEntityActivityEntityId",
                        column: x => x.TrackEntityActivityEntityId,
                        principalTable: "ActivityEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEntity_AthleteEntityId",
                table: "ActivityEntity",
                column: "AthleteEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackPointVO_TrackEntityActivityEntityId",
                table: "TrackPointVO",
                column: "TrackEntityActivityEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackPointVO");

            migrationBuilder.DropTable(
                name: "ActivityEntity");

            migrationBuilder.DropTable(
                name: "Athletes");
        }
    }
}
