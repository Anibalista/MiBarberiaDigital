using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_SGBM.Migrations
{
    /// <inheritdoc />
    public partial class nuevaTransaccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposTransacciones",
                columns: new[] { "IdTipoTransaccion", "Tipo" },
                values: new object[] { 10, "Apertura de Caja" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposTransacciones",
                keyColumn: "IdTipoTransaccion",
                keyValue: 10);
        }
    }
}
