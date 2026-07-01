using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoadReady.API.Migrations
{
    /// <inheritdoc />
    public partial class ReCreateRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Token = table.Column<string>(nullable: false),

                    ExpiryDate = table.Column<DateTime>(nullable: false),

                    CreatedAt = table.Column<DateTime>(nullable: false),

                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_RefreshTokens",
                        x => x.RefreshTokenId);

                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
