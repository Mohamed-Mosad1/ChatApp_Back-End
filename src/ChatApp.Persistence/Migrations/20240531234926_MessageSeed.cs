using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "DateRead", "IsActive", "MessageSend", "ModifiedDate", "RecipientDeleted", "RecipientId", "RecipientUserName", "SenderDeleted", "SenderId", "SenderUserName" },
                values: new object[,]
                {
                    { 1, "test-one", new DateTime(2024, 5, 30, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2529), true, new DateTime(2024, 5, 29, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2539), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Mosaad", false, 1, "Mohamed" },
                    { 2, "test-two", new DateTime(2024, 5, 28, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2544), false, new DateTime(2024, 5, 27, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2545), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Ahmed", false, 2, "Khaled" },
                    { 3, "test-three", new DateTime(2024, 6, 4, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2548), true, new DateTime(2024, 6, 3, 23, 49, 26, 315, DateTimeKind.Utc).AddTicks(2549), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Ali", false, 2, "Hossam" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
