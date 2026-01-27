using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Messages.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                schema: "messages",
                table: "messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_read",
                schema: "messages",
                table: "messages");
        }
    }
}
