using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mi_tension_backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEdadUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "edad",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "edad",
                table: "Usuarios");
        }
    }
}
