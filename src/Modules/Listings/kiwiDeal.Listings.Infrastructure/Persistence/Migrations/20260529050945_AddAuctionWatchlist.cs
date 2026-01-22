using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionWatchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auction_watchlist",
                schema: "listings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    auction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auction_watchlist", x => new { x.user_id, x.auction_id });
                });

            migrationBuilder.CreateIndex(
                name: "ix_auction_watchlist_auction_id",
                schema: "listings",
                table: "auction_watchlist",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "ix_auction_watchlist_user_id",
                schema: "listings",
                table: "auction_watchlist",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auction_watchlist",
                schema: "listings");
        }
    }
}
