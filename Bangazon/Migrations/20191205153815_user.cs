using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "dc3b4603-4277-4bc9-805a-dd3685a91efc", "AQAAAAEAACcQAAAAEG35Jrq11ez+Hg8Wfr/IJ3XlWk2gUmC/wKqfSh3Uw5ZPHFLeTnjvMgraAkclJMLTlA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-fffffffffffg",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "135ad846-8fb0-4b46-999a-681654f32a91", "AQAAAAEAACcQAAAAEJLip34Ow9anikOHZlybXoLXnk4qVptOY33fzUhEKOIHAArLA+OlacrGMTuAuqZsxw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
