using Microsoft.EntityFrameworkCore.Migrations;

namespace ismycommitmessageuseful.Migrations
{
    public partial class LowercaseNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Commits",
                table: "Commits");

            migrationBuilder.RenameTable(
                name: "Commits",
                newName: "commit");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "commit",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "commit",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UsefulCount",
                table: "commit",
                newName: "useful_count");

            migrationBuilder.RenameColumn(
                name: "NotUsefulCount",
                table: "commit",
                newName: "not_useful_count");

            migrationBuilder.RenameColumn(
                name: "DontKnowCount",
                table: "commit",
                newName: "dont_know_count");

            migrationBuilder.AddPrimaryKey(
                name: "commit_id_pkey",
                table: "commit",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "commit_id_pkey",
                table: "commit");

            migrationBuilder.RenameTable(
                name: "commit",
                newName: "Commits");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "Commits",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Commits",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "useful_count",
                table: "Commits",
                newName: "UsefulCount");

            migrationBuilder.RenameColumn(
                name: "not_useful_count",
                table: "Commits",
                newName: "NotUsefulCount");

            migrationBuilder.RenameColumn(
                name: "dont_know_count",
                table: "Commits",
                newName: "DontKnowCount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Commits",
                table: "Commits",
                column: "Id");
        }
    }
}
