using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicantAdmission.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationAverageScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageScore",
                table: "EducationDocuments",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "EducationDocuments");
        }
    }
}
