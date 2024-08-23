using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedMCQOptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BodyType",
                table: "MultipleChoiceQuestionOption",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MultipleChoiceQuestionOption_BodyType_Enum",
                table: "MultipleChoiceQuestionOption",
                sql: "[BodyType] IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MultipleChoiceQuestionOption_BodyType_Enum",
                table: "MultipleChoiceQuestionOption");

            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "MultipleChoiceQuestionOption");
        }
    }
}
