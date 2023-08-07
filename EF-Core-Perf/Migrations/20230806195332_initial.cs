using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_Core_Perf.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestingAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<string>(type: "TEXT", nullable: true),
                    Longitude = table.Column<string>(type: "TEXT", nullable: true),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", nullable: true),
                    Future = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoadId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestingAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestingAreas_Roads_RoadId",
                        column: x => x.RoadId,
                        principalTable: "Roads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestingAreaFeature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RestingAreaEntityId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestingAreaFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestingAreaFeature_RestingAreas_RestingAreaEntityId",
                        column: x => x.RestingAreaEntityId,
                        principalTable: "RestingAreas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestingAreaFeature_RestingAreaEntityId",
                table: "RestingAreaFeature",
                column: "RestingAreaEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RestingAreas_RoadId",
                table: "RestingAreas",
                column: "RoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestingAreaFeature");

            migrationBuilder.DropTable(
                name: "RestingAreas");

            migrationBuilder.DropTable(
                name: "Roads");
        }
    }
}
