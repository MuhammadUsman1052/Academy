using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMathAndScienceAcademy.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260904140000_AddTeachingRolesAndProfilePhotos")]
public partial class AddTeachingRolesAndProfilePhotos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "AssignmentRole", table: "TeachingAssignments", type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Primary");
        migrationBuilder.AddColumn<string>(name: "ProfilePhotoKey", table: "Students", type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<string>(name: "ProfilePhotoContentType", table: "Students", type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "ProfilePhotoUpdatedAt", table: "Students", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<string>(name: "ProfilePhotoKey", table: "Teachers", type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<string>(name: "ProfilePhotoContentType", table: "Teachers", type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "ProfilePhotoUpdatedAt", table: "Teachers", type: "timestamp with time zone", nullable: true);

        migrationBuilder.Sql("""
            WITH ranked AS (
                SELECT "Id", ROW_NUMBER() OVER (
                    PARTITION BY "AcademyId", "ClassId", "SubjectId", "AcademicYear"
                    ORDER BY "CreatedAt", "Id") AS row_number
                FROM "TeachingAssignments"
                WHERE "Status" = 'Active'
            )
            UPDATE "TeachingAssignments" AS assignment
            SET "AssignmentRole" = 'CoTeacher'
            FROM ranked
            WHERE assignment."Id" = ranked."Id" AND ranked.row_number > 1;
            """);

        migrationBuilder.DropIndex(name: "IX_TeachingAssignments_TeacherId_SubjectId_ClassId_AcademicYear", table: "TeachingAssignments");
        migrationBuilder.CreateIndex(name: "IX_TeachingAssignments_AcademyId_ClassId_SubjectId_AcademicYear", table: "TeachingAssignments", columns: new[] { "AcademyId", "ClassId", "SubjectId", "AcademicYear" });
        migrationBuilder.CreateIndex(name: "IX_TeachingAssignments_AcademyId_TeacherId_Status", table: "TeachingAssignments", columns: new[] { "AcademyId", "TeacherId", "Status" });
        migrationBuilder.Sql("""
            CREATE UNIQUE INDEX "IX_TeachingAssignments_ActivePrimary"
            ON "TeachingAssignments" ("AcademyId", "ClassId", "SubjectId", "AcademicYear")
            WHERE "Status" = 'Active' AND "AssignmentRole" = 'Primary';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_TeachingAssignments_ActivePrimary\";");
        migrationBuilder.DropIndex(name: "IX_TeachingAssignments_AcademyId_ClassId_SubjectId_AcademicYear", table: "TeachingAssignments");
        migrationBuilder.DropIndex(name: "IX_TeachingAssignments_AcademyId_TeacherId_Status", table: "TeachingAssignments");
        migrationBuilder.CreateIndex(name: "IX_TeachingAssignments_TeacherId_SubjectId_ClassId_AcademicYear", table: "TeachingAssignments", columns: new[] { "TeacherId", "SubjectId", "ClassId", "AcademicYear" }, unique: true);
        migrationBuilder.DropColumn(name: "AssignmentRole", table: "TeachingAssignments");
        migrationBuilder.DropColumn(name: "ProfilePhotoKey", table: "Students");
        migrationBuilder.DropColumn(name: "ProfilePhotoContentType", table: "Students");
        migrationBuilder.DropColumn(name: "ProfilePhotoUpdatedAt", table: "Students");
        migrationBuilder.DropColumn(name: "ProfilePhotoKey", table: "Teachers");
        migrationBuilder.DropColumn(name: "ProfilePhotoContentType", table: "Teachers");
        migrationBuilder.DropColumn(name: "ProfilePhotoUpdatedAt", table: "Teachers");
    }
}
