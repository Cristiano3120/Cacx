using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CacxServer.Migrations
{
    /// <inheritdoc />
    public partial class ReserveTablePrimaryKeyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservedUserData",
                table: "ReservedUserData",
                column: "EmailHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservedUserData",
                table: "ReservedUserData");
        }
    }
}
