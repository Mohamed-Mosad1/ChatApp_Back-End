using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotoSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublishId",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.InsertData(
                table: "Photos",
                columns: new[] { "Id", "AppUserId", "IsActive", "IsMain", "ModifiedDate", "PublishId", "Url" },
                values: new object[,]
                {
                    { 1, "2b7872a9-6c31-4b56-9b9d-2932d4d7d2d0", true, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "https://xsgames.co/randomusers/assets/avatars/male/1.jpg" },
                    { 2, "2df43ac6-0edc-4d04-bc1c-54a8cca94583", true, true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "https://xsgames.co/randomusers/assets/avatars/male/2.jpg" },
                    { 3, "a735f1fc-c79f-4734-b34f-4c738406c275", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "https://xsgames.co/randomusers/assets/avatars/male/3.jpg" },
                    { 4, "d6290192-24bb-4238-a984-9cf8bac6af05", true, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "https://xsgames.co/randomusers/assets/avatars/male/4.jpg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Photos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Photos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Photos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Photos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "PublishId",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 5, 31, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4620), new DateTime(2024, 5, 30, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4630) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 5, 29, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4635), new DateTime(2024, 5, 28, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4636) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 5, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4639), new DateTime(2024, 6, 4, 17, 48, 23, 738, DateTimeKind.Utc).AddTicks(4640) });
        }
    }
}
