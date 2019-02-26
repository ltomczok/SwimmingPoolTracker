using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class schedule_alternatekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Schedules",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Schedules_Day_Time",
                table: "Schedules",
                columns: new[] { "Day", "Time" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Schedules_Day_Time",
                table: "Schedules");

            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Schedules",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
