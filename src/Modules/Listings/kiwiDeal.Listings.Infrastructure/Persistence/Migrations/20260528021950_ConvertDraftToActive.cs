using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Listings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertDraftToActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE listings.listings SET status = 'Active' WHERE status = 'Draft'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
