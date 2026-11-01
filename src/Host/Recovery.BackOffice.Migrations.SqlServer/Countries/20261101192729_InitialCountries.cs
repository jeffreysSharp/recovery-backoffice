using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recovery.BackOffice.Migrations.SqlServer.Countries
{
    /// <inheritdoc />
    public partial class InitialCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Iso2Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Iso3Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NumericCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    PhoneCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.CheckConstraint("CK_Countries_Iso2", "LEN([Iso2Code]) = 2 AND [Iso2Code] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^A-Z]%'");
                    table.CheckConstraint("CK_Countries_Iso3", "LEN([Iso3Code]) = 3 AND [Iso3Code] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^A-Z]%'");
                    table.CheckConstraint("CK_Countries_Name", "LEN(LTRIM(RTRIM([Name]))) > 0");
                    table.CheckConstraint("CK_Countries_Numeric", "[NumericCode] IS NULL OR (LEN([NumericCode]) = 3 AND [NumericCode] NOT LIKE '%[^0-9]%')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso2Code",
                schema: "catalog",
                table: "Countries",
                column: "Iso2Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso3Code",
                schema: "catalog",
                table: "Countries",
                column: "Iso3Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries",
                schema: "catalog");
        }
    }
}
