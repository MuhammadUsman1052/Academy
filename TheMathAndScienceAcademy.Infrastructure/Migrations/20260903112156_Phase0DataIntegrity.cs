using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMathAndScienceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase0DataIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove invalid historical references before enforcing the new foreign keys.
            migrationBuilder.Sql("""
                DELETE FROM "StudentEnrollments"
                WHERE NOT EXISTS (SELECT 1 FROM "Students" s WHERE s."Id" = "StudentEnrollments"."StudentId")
                   OR NOT EXISTS (SELECT 1 FROM "AcademicClasses" c WHERE c."Id" = "StudentEnrollments"."ClassId")
                   OR NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = "StudentEnrollments"."AcademyId");

                UPDATE "Students"
                SET "UserId" = NULL
                WHERE "UserId" IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM "Users" u WHERE u."Id" = "Students"."UserId");

                DELETE FROM "Students"
                WHERE NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = "Students"."AcademyId");

                DELETE FROM "AcademicClasses"
                WHERE NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = "AcademicClasses"."AcademyId");

                UPDATE "Users"
                SET "AcademyId" = NULL
                WHERE "AcademyId" IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = "Users"."AcademyId");

                DELETE FROM "Users"
                WHERE NOT EXISTS (SELECT 1 FROM "Roles" r WHERE r."Id" = "Users"."RoleId");
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AcademyId",
                table: "Users",
                column: "AcademyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicClasses_Academies_AcademyId",
                table: "AcademicClasses",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEnrollments_AcademicClasses_ClassId",
                table: "StudentEnrollments",
                column: "ClassId",
                principalTable: "AcademicClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEnrollments_Academies_AcademyId",
                table: "StudentEnrollments",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEnrollments_Students_StudentId",
                table: "StudentEnrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Academies_AcademyId",
                table: "Students",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Academies_AcademyId",
                table: "Users",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicClasses_Academies_AcademyId",
                table: "AcademicClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEnrollments_AcademicClasses_ClassId",
                table: "StudentEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEnrollments_Academies_AcademyId",
                table: "StudentEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentEnrollments_Students_StudentId",
                table: "StudentEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Academies_AcademyId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Academies_AcademyId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AcademyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");
        }
    }
}
