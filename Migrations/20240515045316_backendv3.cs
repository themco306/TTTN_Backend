using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class backendv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryDistrict",
                table: "OrderInfos",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryProvince",
                table: "OrderInfos",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryWard",
                table: "OrderInfos",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDistrict",
                table: "OrderInfos");

            migrationBuilder.DropColumn(
                name: "DeliveryProvince",
                table: "OrderInfos");

            migrationBuilder.DropColumn(
                name: "DeliveryWard",
                table: "OrderInfos");
        }
    }
}
