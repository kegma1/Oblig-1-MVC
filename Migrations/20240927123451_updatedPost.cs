using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oblig1.Migrations
{
    public partial class updatedPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Blogs_BlogId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "Posts",
                newName: "blogId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_BlogId",
                table: "Posts",
                newName: "IX_Posts_blogId");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Blogs_blogId",
                table: "Posts",
                column: "blogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Blogs_blogId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "blogId",
                table: "Posts",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_blogId",
                table: "Posts",
                newName: "IX_Posts_BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Blogs_BlogId",
                table: "Posts",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }
    }
}
