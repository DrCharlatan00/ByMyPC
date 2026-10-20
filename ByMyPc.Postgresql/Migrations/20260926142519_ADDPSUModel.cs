using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class ADDPSUModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCs_PSUDbModel_PSUId",
                table: "PCs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PSUDbModel",
                table: "PSUDbModel");

            migrationBuilder.RenameTable(
                name: "PSUDbModel",
                newName: "Psu");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Psu",
                table: "Psu",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PCs_Psu_PSUId",
                table: "PCs",
                column: "PSUId",
                principalTable: "Psu",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCs_Psu_PSUId",
                table: "PCs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Psu",
                table: "Psu");

            migrationBuilder.RenameTable(
                name: "Psu",
                newName: "PSUDbModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PSUDbModel",
                table: "PSUDbModel",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PCs_PSUDbModel_PSUId",
                table: "PCs",
                column: "PSUId",
                principalTable: "PSUDbModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
