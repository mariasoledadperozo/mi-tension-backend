using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mi_tension_backend.Migrations
{
    /// <inheritdoc />
    public partial class PerfilMedicoCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "edad",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "clave",
                table: "Usuarios",
                newName: "Clave");

            migrationBuilder.AddColumn<double>(
                name: "Altura",
                table: "Usuarios",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsFumador",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Peso",
                table: "Usuarios",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TomaMedicacion",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altura",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EsFumador",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TomaMedicacion",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Clave",
                table: "Usuarios",
                newName: "clave");

            migrationBuilder.AddColumn<int>(
                name: "edad",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
