using Microsoft.EntityFrameworkCore.Migrations;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class pool_address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "SwimmingPools",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Address",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Address",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Address",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Address");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "SwimmingPools",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
