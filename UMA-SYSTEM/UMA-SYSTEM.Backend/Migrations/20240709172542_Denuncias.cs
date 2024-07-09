using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMA_SYSTEM.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Denuncias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DNI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CantidadArboles = table.Column<int>(type: "int", nullable: false),
                    TerrenoTenencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Denominacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aldea = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Barrio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Caserio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TiposDenuncia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDenuncia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Denuncias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumExpediente = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuntoReferencia = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DescripcionHechos = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Municipio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Aldea = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Barrio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Caserio = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    TipoDenunciaId = table.Column<int>(type: "int", nullable: true),
                    IdTipoDenuncia = table.Column<int>(type: "int", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: true),
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denuncias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Denuncias_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Denuncias_TiposDenuncia_TipoDenunciaId",
                        column: x => x.TipoDenunciaId,
                        principalTable: "TiposDenuncia",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreArchivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DenunciaId = table.Column<int>(type: "int", nullable: true),
                    IdDenuncia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Anexos_Denuncias_DenunciaId",
                        column: x => x.DenunciaId,
                        principalTable: "Denuncias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_DenunciaId",
                table: "Anexos",
                column: "DenunciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_EstadoId",
                table: "Denuncias",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_TipoDenunciaId",
                table: "Denuncias",
                column: "TipoDenunciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_UsuarioId",
                table: "Solicitudes",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Denuncias");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "TiposDenuncia");
        }
    }
}
