using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMA_SYSTEM.Backend.Migrations
{
    /// <inheritdoc />
    public partial class NuevasSolicitudes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesICF",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Domicilio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PieTablar = table.Column<int>(type: "int", nullable: false),
                    MetrosCubicos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipoMadera = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SitioTenencia = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Denominacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExtensionSuperficial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LimiteNorte = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LimiteSur = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LimiteEste = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LimiteOeste = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: true),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesICF", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesICF_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesICF_EstadoId",
                table: "SolicitudesICF",
                column: "EstadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesICF");
        }
    }
}
