using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicantAdmission.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AdmissionStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<int>(
                name: "Status_tmp",
                table: "ApplicantAdmissions",
                nullable: false,
                defaultValue: 0);

            
            migrationBuilder.Sql(@"
        UPDATE ""ApplicantAdmissions""
        SET ""Status_tmp"" = CASE ""Status""
            WHEN 'Submitted' THEN 0
            WHEN 'InReview' THEN 1
            WHEN 'Accepted' THEN 2
            WHEN 'Rejected' THEN 3
            ELSE 0
        END;
    ");

            
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ApplicantAdmissions");

            
            migrationBuilder.RenameColumn(
                name: "Status_tmp",
                table: "ApplicantAdmissions",
                newName: "Status");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status_tmp",
                table: "ApplicantAdmissions",
                type: "text",
                nullable: false,
                defaultValue: "Submitted");

            migrationBuilder.Sql(@"
        UPDATE ""ApplicantAdmissions""
        SET ""Status_tmp"" = CASE ""Status""
            WHEN 0 THEN 'Submitted'
            WHEN 1 THEN 'InReview'
            WHEN 2 THEN 'Accepted'
            WHEN 3 THEN 'Rejected'
            ELSE 'Submitted'
        END;
    ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ApplicantAdmissions");

            migrationBuilder.RenameColumn(
                name: "Status_tmp",
                table: "ApplicantAdmissions",
                newName: "Status");
        }

    }
}
