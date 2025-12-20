using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockerProject.Migrations
{
    /// <inheritdoc />
    public partial class CommentsAndVoteFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentVote_AspNetUsers_UserId",
                table: "CommentVote");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentVote_Comments_CommentId",
                table: "CommentVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentVote",
                table: "CommentVote");

            migrationBuilder.RenameTable(
                name: "CommentVote",
                newName: "CommentsVotes");

            migrationBuilder.RenameIndex(
                name: "IX_CommentVote_CommentId",
                table: "CommentsVotes",
                newName: "IX_CommentsVotes_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentsVotes",
                table: "CommentsVotes",
                columns: new[] { "UserId", "CommentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsVotes_AspNetUsers_UserId",
                table: "CommentsVotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsVotes_Comments_CommentId",
                table: "CommentsVotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsVotes_AspNetUsers_UserId",
                table: "CommentsVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentsVotes_Comments_CommentId",
                table: "CommentsVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentsVotes",
                table: "CommentsVotes");

            migrationBuilder.RenameTable(
                name: "CommentsVotes",
                newName: "CommentVote");

            migrationBuilder.RenameIndex(
                name: "IX_CommentsVotes_CommentId",
                table: "CommentVote",
                newName: "IX_CommentVote_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentVote",
                table: "CommentVote",
                columns: new[] { "UserId", "CommentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVote_AspNetUsers_UserId",
                table: "CommentVote",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVote_Comments_CommentId",
                table: "CommentVote",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
