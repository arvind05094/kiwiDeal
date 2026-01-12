using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddListingTitleAndBidderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "listing_title",
                schema: "auctions",
                table: "auctions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bidder_name",
                schema: "auctions",
                table: "auction_bids",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "listing_title",
                schema: "auctions",
                table: "auctions");

            migrationBuilder.DropColumn(
                name: "bidder_name",
                schema: "auctions",
                table: "auction_bids");
        }
    }
}
