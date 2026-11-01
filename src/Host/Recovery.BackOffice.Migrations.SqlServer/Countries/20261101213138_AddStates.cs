using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recovery.BackOffice.Migrations.SqlServer.Countries
{
    /// <inheritdoc />
    public partial class AddStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "States",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IbgeCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.CheckConstraint("CK_States_Code", "LEN(LTRIM(RTRIM([Code]))) BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_States_IbgeCode", "[IbgeCode] IS NULL OR (LEN([IbgeCode]) = 2 AND [IbgeCode] NOT LIKE '%[^0-9]%')");
                    table.CheckConstraint("CK_States_Name", "LEN(LTRIM(RTRIM([Name]))) BETWEEN 2 AND 100");
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "catalog",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId_Code",
                schema: "catalog",
                table: "States",
                columns: new[] { "CountryId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId_IbgeCode",
                schema: "catalog",
                table: "States",
                columns: new[] { "CountryId", "IbgeCode" },
                unique: true,
                filter: "[IbgeCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "States",
                schema: "catalog");
        }
    }
}
