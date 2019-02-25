using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class SwimmingPools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PoolId",
                table: "Schedules",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SwimmingPools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ShortName = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwimmingPools", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PoolId",
                table: "Schedules",
                column: "PoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules",
                column: "PoolId",
                principalTable: "SwimmingPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "SwimmingPools");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_PoolId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "PoolId",
                table: "Schedules");
        }
    }
}
