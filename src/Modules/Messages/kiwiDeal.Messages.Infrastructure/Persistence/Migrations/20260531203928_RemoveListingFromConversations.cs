using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Messages.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveListingFromConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_conversations_listing_sender_recipient",
                schema: "messages",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "listing_id",
                schema: "messages",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "listing_title",
                schema: "messages",
                table: "conversations");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_sender_recipient",
                schema: "messages",
                table: "conversations",
                columns: new[] { "sender_id", "recipient_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_conversations_sender_recipient",
                schema: "messages",
                table: "conversations");

            migrationBuilder.AddColumn<Guid>(
                name: "listing_id",
                schema: "messages",
                table: "conversations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "listing_title",
                schema: "messages",
                table: "conversations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_conversations_listing_sender_recipient",
                schema: "messages",
                table: "conversations",
                columns: new[] { "listing_id", "sender_id", "recipient_id" });
        }
    }
}
