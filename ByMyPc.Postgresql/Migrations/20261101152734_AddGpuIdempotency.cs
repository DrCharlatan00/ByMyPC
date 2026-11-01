using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddGpuIdempotency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "idempotencyRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Operation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RequestHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotencyRecords", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_idempotencyRecords_Operation_Key",
                table: "idempotencyRecords",
                columns: new[] { "Operation", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotencyRecords");
        }
    }
}
