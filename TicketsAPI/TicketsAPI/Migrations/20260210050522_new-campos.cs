using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsAPI.Migrations
{
    /// <inheritdoc />
    public partial class newcampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTutor",
                table: "Profesors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "IdProfesor",
                table: "Materias",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProfesorId",
                table: "GradoParalelos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Materias_IdProfesor",
                table: "Materias",
                column: "IdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_GradoParalelos_ProfesorId",
                table: "GradoParalelos",
                column: "ProfesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_GradoParalelos_Profesors_ProfesorId",
                table: "GradoParalelos",
                column: "ProfesorId",
                principalTable: "Profesors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Materias_Profesors_IdProfesor",
                table: "Materias",
                column: "IdProfesor",
                principalTable: "Profesors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GradoParalelos_Profesors_ProfesorId",
                table: "GradoParalelos");

            migrationBuilder.DropForeignKey(
                name: "FK_Materias_Profesors_IdProfesor",
                table: "Materias");

            migrationBuilder.DropIndex(
                name: "IX_Materias_IdProfesor",
                table: "Materias");

            migrationBuilder.DropIndex(
                name: "IX_GradoParalelos_ProfesorId",
                table: "GradoParalelos");

            migrationBuilder.DropColumn(
                name: "IsTutor",
                table: "Profesors");

            migrationBuilder.DropColumn(
                name: "IdProfesor",
                table: "Materias");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "GradoParalelos");
        }
    }
}
