using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMA_SYSTEM.Backend.Migrations
{
    /// <inheritdoc />
    public partial class IntentosPermitidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroIntentos",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroIntentos",
                table: "Usuarios");
        }
    }
}
