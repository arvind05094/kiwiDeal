using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ListingsRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "starting_price",
                schema: "listings",
                table: "listings");

            migrationBuilder.AddColumn<decimal>(
                name: "buy_now_price",
                schema: "listings",
                table: "listings",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                schema: "listings",
                table: "listings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "listing_type",
                schema: "listings",
                table: "listings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "region",
                schema: "listings",
                table: "listings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "listing_watchlist",
                schema: "listings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_watchlist", x => new { x.user_id, x.listing_id });
                    table.ForeignKey(
                        name: "fk_listing_watchlist_listings",
                        column: x => x.listing_id,
                        principalSchema: "listings",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_listings_category",
                schema: "listings",
                table: "listings",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_listings_region",
                schema: "listings",
                table: "listings",
                column: "region");

            migrationBuilder.CreateIndex(
                name: "ix_listing_watchlist_listing_id",
                schema: "listings",
                table: "listing_watchlist",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_watchlist_user_id",
                schema: "listings",
                table: "listing_watchlist",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "listing_watchlist",
                schema: "listings");

            migrationBuilder.DropIndex(
                name: "ix_listings_category",
                schema: "listings",
                table: "listings");

            migrationBuilder.DropIndex(
                name: "ix_listings_region",
                schema: "listings",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "buy_now_price",
                schema: "listings",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "category",
                schema: "listings",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "listing_type",
                schema: "listings",
                table: "listings");

            migrationBuilder.DropColumn(
                name: "region",
                schema: "listings",
                table: "listings");

            migrationBuilder.AddColumn<decimal>(
                name: "starting_price",
                schema: "listings",
                table: "listings",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
