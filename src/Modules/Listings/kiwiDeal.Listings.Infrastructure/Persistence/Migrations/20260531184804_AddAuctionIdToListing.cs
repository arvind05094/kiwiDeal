using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionIdToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "auction_id",
                schema: "listings",
                table: "listings",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auction_id",
                schema: "listings",
                table: "listings");
        }
    }
}
