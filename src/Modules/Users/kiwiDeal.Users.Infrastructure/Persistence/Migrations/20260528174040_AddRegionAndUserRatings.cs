using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kiwiDeal.Users.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionAndUserRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "region",
                schema: "users",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "user_ratings",
                schema: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rater_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ratee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stars = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_ratings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_ratings_ratee_id",
                schema: "users",
                table: "user_ratings",
                column: "ratee_id");

            migrationBuilder.CreateIndex(
                name: "uq_user_ratings_rater_ratee",
                schema: "users",
                table: "user_ratings",
                columns: new[] { "rater_id", "ratee_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_ratings",
                schema: "users");

            migrationBuilder.DropColumn(
                name: "region",
                schema: "users",
                table: "users");
        }
    }
}
