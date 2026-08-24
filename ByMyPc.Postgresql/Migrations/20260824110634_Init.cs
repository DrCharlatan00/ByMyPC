using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByMyPc.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CPUs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    Count_Cores = table.Column<int>(type: "integer", nullable: false),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPUs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GPUs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    VideoMemorySize = table.Column<int>(type: "integer", nullable: false),
                    VideoSlot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GPUs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HDDs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GbSize = table.Column<int>(type: "integer", nullable: false),
                    connector = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDDs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Motherboards",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Socket = table.Column<string>(type: "text", nullable: false),
                    MaxRamSlot = table.Column<int>(type: "integer", nullable: false),
                    MaxRamFrequency = table.Column<int>(type: "integer", nullable: false),
                    MaxCpuFrequency = table.Column<int>(type: "integer", nullable: false),
                    IntegrationGpu = table.Column<bool>(type: "boolean", nullable: false),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false),
                    VideoSlot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motherboards", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PSUDbModel",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PowerWatt = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSUDbModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RAMs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DDRType = table.Column<int>(type: "integer", nullable: false),
                    Frequency = table.Column<int>(type: "integer", nullable: false),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RAMs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PCs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CpuId = table.Column<Guid>(type: "uuid", nullable: true),
                    GpuId = table.Column<Guid>(type: "uuid", nullable: true),
                    MotherboardId = table.Column<Guid>(type: "uuid", nullable: true),
                    PSUId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PCs_CPUs_CpuId",
                        column: x => x.CpuId,
                        principalTable: "CPUs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PCs_GPUs_GpuId",
                        column: x => x.GpuId,
                        principalTable: "GPUs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PCs_Motherboards_MotherboardId",
                        column: x => x.MotherboardId,
                        principalTable: "Motherboards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PCs_PSUDbModel_PSUId",
                        column: x => x.PSUId,
                        principalTable: "PSUDbModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PcHdds",
                columns: table => new
                {
                    PcId = table.Column<Guid>(type: "uuid", nullable: false),
                    HddId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcHdds", x => new { x.PcId, x.HddId });
                    table.ForeignKey(
                        name: "FK_PcHdds_HDDs_HddId",
                        column: x => x.HddId,
                        principalTable: "HDDs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PcHdds_PCs_PcId",
                        column: x => x.PcId,
                        principalTable: "PCs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PcRams",
                columns: table => new
                {
                    PcId = table.Column<Guid>(type: "uuid", nullable: false),
                    RamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcRams", x => new { x.PcId, x.RamId, x.Slot });
                    table.ForeignKey(
                        name: "FK_PcRams_PCs_PcId",
                        column: x => x.PcId,
                        principalTable: "PCs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcRams_RAMs_RamId",
                        column: x => x.RamId,
                        principalTable: "RAMs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PcHdds_HddId",
                table: "PcHdds",
                column: "HddId");

            migrationBuilder.CreateIndex(
                name: "IX_PcRams_RamId",
                table: "PcRams",
                column: "RamId");

            migrationBuilder.CreateIndex(
                name: "IX_PCs_CpuId",
                table: "PCs",
                column: "CpuId");

            migrationBuilder.CreateIndex(
                name: "IX_PCs_GpuId",
                table: "PCs",
                column: "GpuId");

            migrationBuilder.CreateIndex(
                name: "IX_PCs_MotherboardId",
                table: "PCs",
                column: "MotherboardId");

            migrationBuilder.CreateIndex(
                name: "IX_PCs_PSUId",
                table: "PCs",
                column: "PSUId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcHdds");

            migrationBuilder.DropTable(
                name: "PcRams");

            migrationBuilder.DropTable(
                name: "HDDs");

            migrationBuilder.DropTable(
                name: "PCs");

            migrationBuilder.DropTable(
                name: "RAMs");

            migrationBuilder.DropTable(
                name: "CPUs");

            migrationBuilder.DropTable(
                name: "GPUs");

            migrationBuilder.DropTable(
                name: "Motherboards");

            migrationBuilder.DropTable(
                name: "PSUDbModel");
        }
    }
}
