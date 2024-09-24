using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oblig1.Migrations
{
    public partial class AddProfilePictureToBlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Blogs",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Blogs");
        }
    }
}
