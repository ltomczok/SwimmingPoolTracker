using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class pool_MaximumNumberOfLanes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TracksCount",
                table: "SwimmingPools",
                newName: "MaximumNumberOfLanes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaximumNumberOfLanes",
                table: "SwimmingPools",
                newName: "TracksCount");
        }
    }
}
