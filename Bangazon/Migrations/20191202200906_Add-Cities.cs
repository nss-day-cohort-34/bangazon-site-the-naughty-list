using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class AddCities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "81d6aa40-bd41-4dbc-8caf-460ad5b47773", "AQAAAAEAACcQAAAAEBbYBTjF/z5IynXAKcG/UCrTQbs6uQhEBPWN+oSmz5svb1dEjMzk4a9babEpJ/MsXA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-fffffffffffg",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "afa4e55e-a249-46e2-aa7d-db95e7d8eb4b", "AQAAAAEAACcQAAAAEPbB7hqiU58o7WsehVCYJ+yKbuDim0kICUHVIgVYJP80tcaeEm9C8FVeFslJsmJkKw==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "City",
                value: "Nashville");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "City",
                value: "Louisville");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "City",
                value: "Nashville");

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "City",
                value: "Chicago");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "692f01a6-0bfc-4ebf-85a7-6058cfb314a1", "AQAAAAEAACcQAAAAEGbWmCCU+rpBYIWAwr9PhcAkeezlLxsjGoH6odqIm7vNheA3UzCg83jb/viQiuH5MQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-fffffffffffg",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7435966e-8292-4ab2-8b8b-e9152db36e23", "AQAAAAEAACcQAAAAEHqPfYbftz4izY6GLkdvQmTCdmpKvXS70ttR8ZVJLUaDCbEuxZQMiLhPKIM4H3txLA==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "City",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "City",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "City",
                value: null);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "City",
                value: null);
        }
    }
}
