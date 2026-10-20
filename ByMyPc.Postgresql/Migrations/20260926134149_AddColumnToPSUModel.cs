using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToPSUModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Siz",
                table: "PSUDbModel",
                newName: "Size");

            migrationBuilder.AddColumn<bool>(
                name: "IsСertified",
                table: "PSUDbModel",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsСertified",
                table: "PSUDbModel");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "PSUDbModel",
                newName: "Siz");
        }
    }
}
