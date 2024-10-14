using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog4Devs.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Posts_PostsId",
                table: "Comment");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostsId",
                table: "Comment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Posts_PostsId",
                table: "Comment",
                column: "PostsId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Posts_PostsId",
                table: "Comment");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostsId",
                table: "Comment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Posts_PostsId",
                table: "Comment",
                column: "PostsId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
