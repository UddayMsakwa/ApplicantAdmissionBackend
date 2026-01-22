using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApplicantAdmission.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ToEmail = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypes_EducationLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    StudyForm = table.Column<string>(type: "text", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_EducationLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Programs_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Citizenship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applicants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocumentTypeNextLevels",
                columns: table => new
                {
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    NextLevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocumentTypeNextLevels", x => new { x.DocumentTypeId, x.NextLevelId });
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeNextLevels_EducationDocumentTypes_Docu~",
                        column: x => x.DocumentTypeId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationDocumentTypeNextLevels_EducationLevels_NextLevelId",
                        column: x => x.NextLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantAdmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantAdmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantAdmissions_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantAdmissions_Users_ManagerUserId",
                        column: x => x.ManagerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentKind = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicantAdmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionPrograms_ApplicantAdmissions_ApplicantAdmissionId",
                        column: x => x.ApplicantAdmissionId,
                        principalTable: "ApplicantAdmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdmissionPrograms_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentScans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentScans_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationDocuments_Documents_Id",
                        column: x => x.Id,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationDocuments_EducationDocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "EducationDocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassportDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Series = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    IssuedBy = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BirthPlace = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassportDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassportDocuments_Documents_Id",
                        column: x => x.Id,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "IsActive", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000001"), "admin@test.local", "Seeded Admin", true, "AAAAAAAAAAAAAAAAAAAAAA==.EnhHDOCZohHrqGP85zO7Zipwzs4q0OhgUwZQysGSz78=", 0 },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000002"), "mgr1@test.local", "Seeded Manager 1", true, "AQEBAQEBAQEBAQEBAQEBAQ==.T2OFoGXsyB+4m6EU8Cqe+ZayYvxhQGGdxiuZvxXDUYY=", 2 },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000003"), "head1@test.local", "Seeded Head 1", true, "AgICAgICAgICAgICAgICAg==.J+1k/wkwqW+NmBShThK3HzP0VTbZ9il8VemNnUxuy94=", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPrograms_ApplicantAdmissionId_Priority",
                table: "AdmissionPrograms",
                columns: new[] { "ApplicantAdmissionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPrograms_ApplicantAdmissionId_ProgramId",
                table: "AdmissionPrograms",
                columns: new[] { "ApplicantAdmissionId", "ProgramId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPrograms_ProgramId",
                table: "AdmissionPrograms",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantAdmissions_ApplicantId",
                table: "ApplicantAdmissions",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantAdmissions_LastModifiedAt",
                table: "ApplicantAdmissions",
                column: "LastModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantAdmissions_ManagerUserId",
                table: "ApplicantAdmissions",
                column: "ManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_UserId",
                table: "Applicants",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ApplicantId",
                table: "Documents",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentScans_DocumentId",
                table: "DocumentScans",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentScans_FileId",
                table: "DocumentScans",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocuments_DocumentTypeId",
                table: "EducationDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypeNextLevels_NextLevelId",
                table: "EducationDocumentTypeNextLevels",
                column: "NextLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentTypes_LevelId",
                table: "EducationDocumentTypes",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_Code",
                table: "Programs",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_FacultyId",
                table: "Programs",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_Language",
                table: "Programs",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_LevelId",
                table: "Programs",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_StudyForm",
                table: "Programs",
                column: "StudyForm");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionPrograms");

            migrationBuilder.DropTable(
                name: "DocumentScans");

            migrationBuilder.DropTable(
                name: "EducationDocuments");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypeNextLevels");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PassportDocuments");

            migrationBuilder.DropTable(
                name: "ApplicantAdmissions");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "EducationDocumentTypes");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
