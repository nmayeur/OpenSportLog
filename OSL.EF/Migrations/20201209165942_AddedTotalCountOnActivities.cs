using Microsoft.EntityFrameworkCore.Migrations;

namespace OSL.EF.Migrations
{
    public partial class AddedTotalCountOnActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TracksPointsCount",
                table: "ActivityEntity",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TracksPointsCount",
                table: "ActivityEntity");
        }
    }
}
