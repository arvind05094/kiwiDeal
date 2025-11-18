using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auctions");

            migrationBuilder.CreateTable(
                name: "auctions",
                schema: "auctions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starting_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    current_highest_bid = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    current_highest_bidder_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auctions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auction_bids",
                schema: "auctions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bidder_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    auction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auction_bids", x => x.id);
                    table.ForeignKey(
                        name: "fk_auction_bids_auctions",
                        column: x => x.auction_id,
                        principalSchema: "auctions",
                        principalTable: "auctions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auction_bids_auction_id",
                schema: "auctions",
                table: "auction_bids",
                column: "auction_id");

            migrationBuilder.CreateIndex(
                name: "ix_auctions_listing_id",
                schema: "auctions",
                table: "auctions",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_auctions_seller_id",
                schema: "auctions",
                table: "auctions",
                column: "seller_id");

            migrationBuilder.CreateIndex(
                name: "ix_auctions_status",
                schema: "auctions",
                table: "auctions",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auction_bids",
                schema: "auctions");

            migrationBuilder.DropTable(
                name: "auctions",
                schema: "auctions");
        }
    }
}
