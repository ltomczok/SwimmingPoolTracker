using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class pool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SwimmingPools",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SwimmingPools");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "SwimmingPools",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseTime",
                table: "SwimmingPools",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "SwimmingPools",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OpenTime",
                table: "SwimmingPools",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "SwimmingPools",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PoolId",
                table: "Schedules",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SwimmingPools",
                table: "SwimmingPools",
                column: "ShortName");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules",
                column: "PoolId",
                principalTable: "SwimmingPools",
                principalColumn: "ShortName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SwimmingPools",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "SwimmingPools");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "SwimmingPools",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SwimmingPools",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "PoolId",
                table: "Schedules",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SwimmingPools",
                table: "SwimmingPools",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_SwimmingPools_PoolId",
                table: "Schedules",
                column: "PoolId",
                principalTable: "SwimmingPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
