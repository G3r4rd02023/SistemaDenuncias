using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMA_SYSTEM.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Sesiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SesionActiva",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SesionActiva",
                table: "Usuarios");
        }
    }
}
