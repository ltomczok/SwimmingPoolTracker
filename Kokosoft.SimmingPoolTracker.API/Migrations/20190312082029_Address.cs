using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kokosoft.SimmingPoolTracker.API.Migrations
{
    public partial class Address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "SwimmingPools",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SwimmingPools_AddressId",
                table: "SwimmingPools",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SwimmingPools_Address_AddressId",
                table: "SwimmingPools");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_SwimmingPools_AddressId",
                table: "SwimmingPools");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "SwimmingPools");
        }
    }
}
