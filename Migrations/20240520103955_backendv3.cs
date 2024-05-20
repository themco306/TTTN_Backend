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
            migrationBuilder.DropForeignKey(
                name: "FK_CouponUsages_Coupons_CouponId1",
                table: "CouponUsages");

            migrationBuilder.DropIndex(
                name: "IX_CouponUsages_CouponId1",
                table: "CouponUsages");

            migrationBuilder.DropColumn(
                name: "CouponId1",
                table: "CouponUsages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CouponId1",
                table: "CouponUsages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsages_CouponId1",
                table: "CouponUsages",
                column: "CouponId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CouponUsages_Coupons_CouponId1",
                table: "CouponUsages",
                column: "CouponId1",
                principalTable: "Coupons",
                principalColumn: "Id");
        }
    }
}
