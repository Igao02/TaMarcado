using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaMarcado.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoriesAndPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorie",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), "Barbeiro" },
                    { new Guid("11111111-0000-0000-0000-000000000002"), "Psicólogo" },
                    { new Guid("11111111-0000-0000-0000-000000000003"), "Personal Trainer" },
                    { new Guid("11111111-0000-0000-0000-000000000004"), "Dentista" },
                    { new Guid("11111111-0000-0000-0000-000000000005"), "Tatuador" },
                    { new Guid("11111111-0000-0000-0000-000000000006"), "Manicure / Pedicure" },
                    { new Guid("11111111-0000-0000-0000-000000000007"), "Nutricionista" },
                    { new Guid("11111111-0000-0000-0000-000000000008"), "Fisioterapeuta" },
                    { new Guid("11111111-0000-0000-0000-000000000009"), "Esteticista" },
                    { new Guid("11111111-0000-0000-0000-000000000010"), "Outros" }
                });

            migrationBuilder.InsertData(
                table: "Plan",
                columns: new[] { "Id", "Active", "LimitMonthlySchedulings", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("22222222-0000-0000-0000-000000000001"), true, 20, "Grátis", 0m },
                    { new Guid("22222222-0000-0000-0000-000000000002"), true, null, "Pro", 14.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Categorie",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Plan",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000002"));
        }
    }
}
