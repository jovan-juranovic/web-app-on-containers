using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppOnDocker.Infrastructure.ApplicationMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "category_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "CategoryType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: false),
                    CategoryTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_CategoryType_CategoryTypeId",
                        column: x => x.CategoryTypeId,
                        principalTable: "CategoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoryType",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, "Azure" },
                    { 2, "AWS" },
                    { 3, "GCP" },
                    { 4, "Heroku" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryTypeId",
                table: "Category",
                column: "CategoryTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "CategoryType");

            migrationBuilder.DropSequence(
                name: "category_hilo");
        }
    }
}
