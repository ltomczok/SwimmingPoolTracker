using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class schedule_time_maxlenght : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Schedules",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Time",
                table: "Schedules",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);
        }
    }
}
