using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMathAndScienceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUsertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademyId",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademyId",
                table: "Users");
        }
    }
}
