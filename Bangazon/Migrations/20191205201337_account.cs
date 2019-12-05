using Microsoft.EntityFrameworkCore.Migrations;

namespace Bangazon.Migrations
{
    public partial class account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e4cb8184-853d-4b9d-a795-6063d030c2a0", "AQAAAAEAACcQAAAAEO+B98wNAoW7+tjUPd01roYHppIb5Z3vWHfgIKlFQC7QYojbduGGdMWhMJUc41/zoQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-fffffffffffg",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b3a53d70-664c-41e7-80c8-8546b133edb1", "AQAAAAEAACcQAAAAEJbF93pLvoJC73G9VAFIN+oUa92tQjw6G8bxECA1Tk6CxYDdu8Fxm1jnqjFHIBEPWw==" });
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
