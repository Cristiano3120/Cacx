using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CacxServer.Migrations
{
    /// <inheritdoc />
    public partial class ReserveTableUsernameUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReservedUserData_Username",
                table: "ReservedUserData");

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUserData_Username",
                table: "ReservedUserData",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReservedUserData_Username",
                table: "ReservedUserData");

            migrationBuilder.CreateIndex(
                name: "IX_ReservedUserData_Username",
                table: "ReservedUserData",
                column: "Username",
                unique: true);
        }
    }
}
