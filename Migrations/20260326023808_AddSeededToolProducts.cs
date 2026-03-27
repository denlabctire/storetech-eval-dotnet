using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace storetech_eval_dotnet.Migrations
{
    /// <inheritdoc />
    public partial class AddSeededToolProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "IsActive", "Name", "Sku" },
                values: new object[,]
                {
                    { 3, true, "StoreTech Claw Hammer", "SKU-HAMMER-001" },
                    { 4, true, "StoreTech Screwdriver Set", "SKU-SCREWDRIVER-001" },
                    { 5, true, "StoreTech Hand Saw", "SKU-SAW-001" }
                });

            migrationBuilder.InsertData(
                table: "PriceInfos",
                columns: new[] { "Id", "Amount", "CurrencyCode", "EffectiveFromUtc", "EffectiveToUtc", "IsActive", "ProductId" },
                values: new object[,]
                {
                    { 3, 24.99m, "CAD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 3 },
                    { 4, 19.99m, "CAD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 4 },
                    { 5, 29.99m, "CAD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceInfos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PriceInfos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PriceInfos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
