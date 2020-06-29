using Microsoft.EntityFrameworkCore.Migrations;

namespace OSL.EF.Migrations
{
    public partial class Addedaveragesonactivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cadence",
                table: "ActivityEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HeartRate",
                table: "ActivityEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "ActivityEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "ActivityEntity",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cadence",
                table: "ActivityEntity");

            migrationBuilder.DropColumn(
                name: "HeartRate",
                table: "ActivityEntity");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "ActivityEntity");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "ActivityEntity");
        }
    }
}
