using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicantAdmission.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataAndFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EducationLevels",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "Bachelor" });

            migrationBuilder.InsertData(
                table: "Faculties",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "Engineering" });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "Role" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), "manager@test.com", "Admin Manager", "hashed_pass", "Admin" });

            migrationBuilder.InsertData(
                table: "Programs",
                columns: new[] { "Id", "FacultyId", "LevelId", "Name" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new Guid("22222222-2222-2222-2222-222222222222"), "Computer Science" });

            migrationBuilder.InsertData(
                table: "AdmissionPrograms",
                columns: new[] { "Id", "ProgramId" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("33333333-3333-3333-3333-333333333333") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AdmissionPrograms",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Programs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "EducationLevels",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Faculties",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
