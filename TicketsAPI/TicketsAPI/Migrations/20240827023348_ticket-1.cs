using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsAPI.Migrations
{
    /// <inheritdoc />
    public partial class ticket1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Solicituds",
                columns: table => new
                {
                    IdSolicitud = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipoSolicitud = table.Column<int>(type: "int", nullable: false),
                    DescripcionSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Justificativo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estadoSolicitud = table.Column<int>(type: "int", nullable: false),
                    DetalleGestion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaGestion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuario = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicituds", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_Solicituds_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicituds_IdUsuario",
                table: "Solicituds",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Solicituds");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
