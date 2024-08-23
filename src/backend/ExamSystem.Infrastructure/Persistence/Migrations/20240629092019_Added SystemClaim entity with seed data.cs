using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExamSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedSystemClaimentitywithseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemClaims", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SystemClaims",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("1ef4babe-2dfa-ca2d-eb41-08dc8937be87"), null, "InternalUser" },
                    { new Guid("3bf76b81-16fb-cf73-09d8-08dc8937d04b"), null, "QuestionEdit" },
                    { new Guid("3eaa627b-e56f-c685-235d-08dc8937c081"), null, "ViewDashboard" },
                    { new Guid("4e70efc8-77af-c047-2c9a-08dc8937ca23"), null, "QuestionCreate" },
                    { new Guid("589e7ea7-754e-c471-f4e3-08dc8937e29d"), null, "ManageLog" },
                    { new Guid("61dc3de3-65a1-c21d-da2d-08dc8937daa7"), null, "ExamView" },
                    { new Guid("684433eb-bf84-cc23-f24a-08dc8937ded9"), null, "ExamDelete" },
                    { new Guid("7990988a-cf75-ce00-cb47-08dc8937c377"), null, "ManageMemberClaim" },
                    { new Guid("844b3328-080f-c935-ffea-08dc8937c5b2"), null, "ManageMember" },
                    { new Guid("abf0c086-0c63-c906-7dca-08dc8937c7e8"), null, "ManageQuestion" },
                    { new Guid("b7842991-b887-cfc5-d19b-08dc8937d37f"), null, "QuestionDelete" },
                    { new Guid("c32ad88e-09f7-c6f0-ce5b-08dc8937bc4f"), null, "Member" },
                    { new Guid("ccfd2634-ada3-cc17-d019-08dc8937d82f"), null, "ExamCreate" },
                    { new Guid("dde5d028-8ce1-cc74-c2ad-08dc8937d5f6"), null, "ManageExam" },
                    { new Guid("e262c5dc-94e5-c8f4-8e30-08dc8937dce0"), null, "ExamEdit" },
                    { new Guid("ff3058a6-bcf3-c296-ba35-08dc8937cd57"), null, "QuestionView" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemClaims");
        }
    }
}
