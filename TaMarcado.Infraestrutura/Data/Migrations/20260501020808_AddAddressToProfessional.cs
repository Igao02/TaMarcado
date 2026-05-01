using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaMarcado.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Professional",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Professional");
        }
    }
}
