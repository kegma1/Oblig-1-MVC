using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oblig1.Migrations
{
    public partial class RenameDesctiptionToDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "desctiption",
                table: "Blogs",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Blogs",
                newName: "desctiption");
        }
    }
}
