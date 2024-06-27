using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Added_Groups_And_Connections_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_Connections_Groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "Groups",
                        principalColumn: "Name");
                });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 26, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6506), new DateTime(2024, 6, 25, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6512) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 24, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6519), new DateTime(2024, 6, 23, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6519) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 7, 1, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6524), new DateTime(2024, 6, 30, 16, 51, 13, 1, DateTimeKind.Utc).AddTicks(6524) });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_GroupName",
                table: "Connections",
                column: "GroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 15, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7279), new DateTime(2024, 6, 14, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7286) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 13, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7295), new DateTime(2024, 6, 12, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7297) });

            migrationBuilder.UpdateData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateRead", "MessageSend" },
                values: new object[] { new DateTime(2024, 6, 20, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7304), new DateTime(2024, 6, 19, 23, 43, 45, 272, DateTimeKind.Utc).AddTicks(7305) });
        }
    }
}
