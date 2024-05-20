using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class backendv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "PaidOrders",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "PaidOrders",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
