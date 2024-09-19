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
                name: "FieldTypes",
                columns: table => new
                {
                    IdFieldType = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.IdFieldType);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    IdForm = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.IdForm);
                });

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
                name: "FilledForms",
                columns: table => new
                {
                    IdFilledForm = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilledForms", x => x.IdFilledForm);
                    table.ForeignKey(
                        name: "FK_FilledForms_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "IdForm",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormGroups",
                columns: table => new
                {
                    IdFormGroup = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormId = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormGroups", x => x.IdFormGroup);
                    table.ForeignKey(
                        name: "FK_FormGroups_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "IdForm",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "FormFields",
                columns: table => new
                {
                    IdFormField = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    FieldTypeId = table.Column<long>(type: "bigint", nullable: false),
                    FormGroupId = table.Column<long>(type: "bigint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFields", x => x.IdFormField);
                    table.ForeignKey(
                        name: "FK_FormFields_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldTypes",
                        principalColumn: "IdFieldType",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormFields_FormGroups_FormGroupId",
                        column: x => x.FormGroupId,
                        principalTable: "FormGroups",
                        principalColumn: "IdFormGroup",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    IdOption = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFieldIdFormField = table.Column<long>(type: "bigint", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.IdOption);
                    table.ForeignKey(
                        name: "FK_Options_FormFields_FormFieldIdFormField",
                        column: x => x.FormFieldIdFormField,
                        principalTable: "FormFields",
                        principalColumn: "IdFormField");
                });

            migrationBuilder.CreateTable(
                name: "FilledFormField",
                columns: table => new
                {
                    IdFilledFormField = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsChecked = table.Column<bool>(type: "bit", nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FilledFormId = table.Column<long>(type: "bigint", nullable: false),
                    SelectedOptionId = table.Column<long>(type: "bigint", nullable: true),
                    FormFieldIdFormField = table.Column<long>(type: "bigint", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateRegister = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpRegister = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", maxLength: 100, nullable: true),
                    IpModification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateDelete = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpDelete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilledFormField", x => x.IdFilledFormField);
                    table.ForeignKey(
                        name: "FK_FilledFormField_FilledForms_FilledFormId",
                        column: x => x.FilledFormId,
                        principalTable: "FilledForms",
                        principalColumn: "IdFilledForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilledFormField_FormFields_FormFieldIdFormField",
                        column: x => x.FormFieldIdFormField,
                        principalTable: "FormFields",
                        principalColumn: "IdFormField");
                    table.ForeignKey(
                        name: "FK_FilledFormField_Options_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "Options",
                        principalColumn: "IdOption");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilledFormField_FilledFormId",
                table: "FilledFormField",
                column: "FilledFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FilledFormField_FormFieldIdFormField",
                table: "FilledFormField",
                column: "FormFieldIdFormField");

            migrationBuilder.CreateIndex(
                name: "IX_FilledFormField_SelectedOptionId",
                table: "FilledFormField",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_FilledForms_FormId",
                table: "FilledForms",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_FieldTypeId",
                table: "FormFields",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_FormGroupId",
                table: "FormFields",
                column: "FormGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FormGroups_FormId",
                table: "FormGroups",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_FormFieldIdFormField",
                table: "Options",
                column: "FormFieldIdFormField");

            migrationBuilder.CreateIndex(
                name: "IX_Solicituds_IdUsuario",
                table: "Solicituds",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilledFormField");

            migrationBuilder.DropTable(
                name: "Solicituds");

            migrationBuilder.DropTable(
                name: "FilledForms");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "FormFields");

            migrationBuilder.DropTable(
                name: "FieldTypes");

            migrationBuilder.DropTable(
                name: "FormGroups");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
