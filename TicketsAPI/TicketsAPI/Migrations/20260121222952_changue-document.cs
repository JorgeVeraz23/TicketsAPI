using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsAPI.Migrations
{
    /// <inheritdoc />
    public partial class changuedocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documento_EstudianteId",
                table: "Documento");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Documento",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Documento",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Aprobado",
                table: "Documento",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Documento",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevision",
                table: "Documento",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "HashArchivo",
                table: "Documento",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Documento",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacion",
                table: "Documento",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TamanoBytes",
                table: "Documento",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TipoDocumentoId",
                table: "Documento",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRevision",
                table: "Documento",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Vigente = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioEliminacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documento_EstudianteId_TipoDocumentoId",
                table: "Documento",
                columns: new[] { "EstudianteId", "TipoDocumentoId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_TipoDocumentoId",
                table: "Documento",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoDocumento_Codigo",
                table: "TipoDocumento",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documento_TipoDocumento_TipoDocumentoId",
                table: "Documento",
                column: "TipoDocumentoId",
                principalTable: "TipoDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documento_TipoDocumento_TipoDocumentoId",
                table: "Documento");

            migrationBuilder.DropTable(
                name: "TipoDocumento");

            migrationBuilder.DropIndex(
                name: "IX_Documento_EstudianteId_TipoDocumentoId",
                table: "Documento");

            migrationBuilder.DropIndex(
                name: "IX_Documento_TipoDocumentoId",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "Aprobado",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "FechaRevision",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "HashArchivo",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "Observacion",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "TamanoBytes",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoId",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "UsuarioRevision",
                table: "Documento");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Documento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Documento",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "IX_Documento_EstudianteId",
                table: "Documento",
                column: "EstudianteId");
        }
    }
}
