using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedLikeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLikes",
                columns: table => new
                {
                    SourceUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LikedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikes", x => new { x.SourceUserId, x.LikedUserId });
                    table.ForeignKey(
                        name: "FK_UserLikes_AspNetUsers_LikedUserId",
                        column: x => x.LikedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLikes_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 15, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1809), new DateTime(2024, 6, 14, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1819) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 13, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1824), new DateTime(2024, 6, 12, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1825) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 20, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1828), new DateTime(2024, 6, 19, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1829) });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikes_LikedUserId",
                table: "UserLikes",
                column: "LikedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLikes");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 2, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3010), new DateTime(2024, 6, 1, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3018) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 5, 31, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3021), new DateTime(2024, 5, 30, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3022) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 7, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3025), new DateTime(2024, 6, 6, 16, 49, 26, 629, DateTimeKind.Utc).AddTicks(3026) });
        }
    }
}
