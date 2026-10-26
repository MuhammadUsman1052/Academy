using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMathAndScienceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAcademyForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "RolePermissions"
                WHERE "RoleId" IN (
                    SELECT r."Id" FROM "Roles" r
                    WHERE r."AcademyId" IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = r."AcademyId")
                );

                DELETE FROM "Roles"
                WHERE "AcademyId" IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM "Academies" a WHERE a."Id" = "Roles"."AcademyId");
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_AcademyId",
                table: "Roles",
                column: "AcademyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Academies_AcademyId",
                table: "Roles",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Academies_AcademyId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_AcademyId",
                table: "Roles");
        }
    }
}
