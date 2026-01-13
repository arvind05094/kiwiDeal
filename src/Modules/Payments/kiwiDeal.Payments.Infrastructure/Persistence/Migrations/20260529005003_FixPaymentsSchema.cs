using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Payments.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "winner_id",
                schema: "payments",
                table: "payments",
                newName: "listing_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "auction_id",
                schema: "payments",
                table: "payments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "buyer_id",
                schema: "payments",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "payment_type",
                schema: "payments",
                table: "payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "buyer_id",
                schema: "payments",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "payment_type",
                schema: "payments",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "listing_id",
                schema: "payments",
                table: "payments",
                newName: "winner_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "auction_id",
                schema: "payments",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
