using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Messages_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 15, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7279), new DateTime(2024, 6, 14, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7286), "2", "1" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 13, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7295), new DateTime(2024, 6, 12, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7297), "1", "2" });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 20, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7304), new DateTime(2024, 6, 19, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7305), "1", "2" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_RecipientId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Messages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "RecipientId",
                table: "Messages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 15, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1809), new DateTime(2024, 6, 14, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1819), 2, 1 });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 13, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1824), new DateTime(2024, 6, 12, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1825), 1, 2 });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend", "RecipientId", "SenderId" },
                values: new object[] { new DateTime(2024, 6, 20, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1828), new DateTime(2024, 6, 19, 16, 5, 58, 433, DateTimeKind.Utc).AddTicks(1829), 1, 2 });
        }
    }
}
