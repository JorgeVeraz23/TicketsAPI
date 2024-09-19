using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketsAPI.Migrations
{
    /// <inheritdoc />
    public partial class ticket2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Options_FormFields_FormFieldIdFormField",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Options_FormFieldIdFormField",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "FormFieldIdFormField",
                table: "Options");

            migrationBuilder.AddColumn<long>(
                name: "idFormField",
                table: "Options",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Options_idFormField",
                table: "Options",
                column: "idFormField");

            migrationBuilder.AddForeignKey(
                name: "FK_Options_FormFields_idFormField",
                table: "Options",
                column: "idFormField",
                principalTable: "FormFields",
                principalColumn: "IdFormField",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Options_FormFields_idFormField",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Options_idFormField",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "idFormField",
                table: "Options");

            migrationBuilder.AddColumn<long>(
                name: "FormFieldIdFormField",
                table: "Options",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Options_FormFieldIdFormField",
                table: "Options",
                column: "FormFieldIdFormField");

            migrationBuilder.AddForeignKey(
                name: "FK_Options_FormFields_FormFieldIdFormField",
                table: "Options",
                column: "FormFieldIdFormField",
                principalTable: "FormFields",
                principalColumn: "IdFormField");
        }
    }
}
