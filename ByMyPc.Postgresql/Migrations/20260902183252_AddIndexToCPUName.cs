using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToCPUName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CPUs_Name",
                table: "CPUs",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CPUs_Name",
                table: "CPUs");
        }
    }
}
