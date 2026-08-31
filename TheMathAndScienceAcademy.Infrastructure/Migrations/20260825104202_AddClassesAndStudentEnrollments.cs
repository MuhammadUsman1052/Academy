using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMathAndScienceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassesAndStudentEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicClasses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Grade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Section = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AcademyId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentEnrollments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClassId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AcademyId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AcademicYear = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEnrollments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicClasses_AcademyId_Grade_Section_AcademicYear",
                table: "AcademicClasses",
                columns: new[] { "AcademyId", "Grade", "Section", "AcademicYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademyId",
                table: "StudentEnrollments",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_ClassId",
                table: "StudentEnrollments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId_AcademicYear_AcademyId_Status",
                table: "StudentEnrollments",
                columns: new[] { "StudentId", "AcademicYear", "AcademyId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicClasses");

            migrationBuilder.DropTable(
                name: "StudentEnrollments");
        }
    }
}
