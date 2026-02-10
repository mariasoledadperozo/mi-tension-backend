using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mi_tension_backend.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionRecordatorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dias",
                table: "Recordatorios",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dias",
                table: "Recordatorios");
        }
    }
}
