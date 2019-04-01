using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class start_end_time : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_Day_Time",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Schedules",
                newName: "StartTime");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Schedules",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_Day_StartTime",
                table: "Schedules",
                columns: new[] { "Day", "StartTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_Day_StartTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Schedules",
                newName: "Time");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_Day_Time",
                table: "Schedules",
                columns: new[] { "Day", "Time" });
        }
    }
}
