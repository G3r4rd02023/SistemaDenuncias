using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMA_SYSTEM.Backend.Migrations
{
    /// <inheritdoc />
    public partial class SolicitudesICF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Solicitudes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Solicitudes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Solicitudes");
        }
    }
}
