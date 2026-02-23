using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Messages.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxRetryTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error",
                schema: "messages",
                table: "outbox_messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "retry_count",
                schema: "messages",
                table: "outbox_messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error",
                schema: "messages",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "retry_count",
                schema: "messages",
                table: "outbox_messages");
        }
    }
}
