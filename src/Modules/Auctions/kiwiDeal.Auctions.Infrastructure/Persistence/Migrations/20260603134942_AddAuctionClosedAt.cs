using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionClosedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "closed_at",
                schema: "auctions",
                table: "auctions",
                type: "timestamptz",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closed_at",
                schema: "auctions",
                table: "auctions");
        }
    }
}
