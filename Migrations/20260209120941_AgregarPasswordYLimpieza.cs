using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mi_tension_backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPasswordYLimpieza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recordatorios_Usuarios_userId",
                table: "Recordatorios");

            migrationBuilder.DropIndex(
                name: "IX_Recordatorios_userId",
                table: "Recordatorios");

            migrationBuilder.AddColumn<string>(
                name: "clave",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Recordatorios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorios_UsuarioId",
                table: "Recordatorios",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recordatorios_Usuarios_UsuarioId",
                table: "Recordatorios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recordatorios_Usuarios_UsuarioId",
                table: "Recordatorios");

            migrationBuilder.DropIndex(
                name: "IX_Recordatorios_UsuarioId",
                table: "Recordatorios");

            migrationBuilder.DropColumn(
                name: "clave",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Recordatorios");

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorios_userId",
                table: "Recordatorios",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recordatorios_Usuarios_userId",
                table: "Recordatorios",
                column: "userId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
