using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Messages.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDenormalizedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sender_name",
                schema: "messages",
                table: "messages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "listing_title",
                schema: "messages",
                table: "conversations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "recipient_name",
                schema: "messages",
                table: "conversations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sender_name",
                schema: "messages",
                table: "conversations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sender_name",
                schema: "messages",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "listing_title",
                schema: "messages",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "recipient_name",
                schema: "messages",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "sender_name",
                schema: "messages",
                table: "conversations");
        }
    }
}
