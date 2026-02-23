using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "auctions",
                table: "auctions",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "auctions",
                table: "auctions");
        }
    }
}
