using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_SGBM.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionNiveles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 3,
                column: "Nivel",
                value: "Barbero Senior");

            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 4,
                column: "Nivel",
                value: "Barbero Junior");

            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 5,
                column: "Nivel",
                value: "Administrativo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 3,
                column: "Nivel",
                value: "Responsable");

            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 4,
                column: "Nivel",
                value: "Barbero");

            migrationBuilder.UpdateData(
                table: "Niveles",
                keyColumn: "IdNivel",
                keyValue: 5,
                column: "Nivel",
                value: "Invitado");
        }
    }
}
