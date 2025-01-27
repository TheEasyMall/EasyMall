using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyMall.DALs.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldTable_Cart_ProductPriceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductPriceId",
                table: "Carts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductPriceId",
                table: "Carts",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductPrices_ProductPriceId",
                table: "Carts",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductPrices_ProductPriceId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductPriceId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "Carts");
        }
    }
}
