using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class pool_trackscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TracksCount",
                table: "SwimmingPools",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TracksCount",
                table: "SwimmingPools");
        }
    }
}
