using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oblig1.Migrations
{
    public partial class AddUserUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowersId",
                table: "UserUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowingId",
                table: "UserUser");

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowersId",
                table: "UserUser",
                column: "FollowersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowingId",
                table: "UserUser",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowersId",
                table: "UserUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowingId",
                table: "UserUser");

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowersId",
                table: "UserUser",
                column: "FollowersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_AspNetUsers_FollowingId",
                table: "UserUser",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
