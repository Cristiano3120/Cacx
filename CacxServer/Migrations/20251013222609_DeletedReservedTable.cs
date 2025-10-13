using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CacxServer.Migrations
{
    /// <inheritdoc />
    public partial class DeletedReservedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedUserData");

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Verified",
                table: "Users",
                column: "Verified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Verified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ReservedUserData",
                columns: table => new
                {
                    EmailHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedUserData", x => x.EmailHash);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUserData_EmailHash",
                table: "ReservedUserData",
                column: "EmailHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUserData_Username",
                table: "ReservedUserData",
                column: "Username");
        }
    }
}
