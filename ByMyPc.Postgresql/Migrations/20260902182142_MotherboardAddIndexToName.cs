using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class MotherboardAddIndexToName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Motherboards_Name",
                table: "Motherboards",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Motherboards_Name",
                table: "Motherboards");
        }
    }
}
