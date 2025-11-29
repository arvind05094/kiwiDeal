using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Auctions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionsOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "auctions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_on = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    processed_on = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on",
                schema: "auctions",
                table: "outbox_messages",
                column: "processed_on",
                filter: "processed_on IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "auctions");
        }
    }
}
