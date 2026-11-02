using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recovery.BackOffice.Migrations.SqlServer.Countries
{
    /// <inheritdoc />
    public partial class AddAgencyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgencyTypes",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AS"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyTypes", x => x.Id);
                    table.CheckConstraint("CK_AgencyTypes_Name", "LEN(LTRIM(RTRIM([Name]))) BETWEEN 3 AND 100");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencyTypes_Name",
                schema: "catalog",
                table: "AgencyTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgencyTypes",
                schema: "catalog");
        }
    }
}
