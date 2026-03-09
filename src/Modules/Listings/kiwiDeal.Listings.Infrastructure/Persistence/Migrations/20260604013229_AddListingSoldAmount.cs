using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddListingSoldAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "sold_amount",
                schema: "listings",
                table: "listings",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sold_amount",
                schema: "listings",
                table: "listings");
        }
    }
}
