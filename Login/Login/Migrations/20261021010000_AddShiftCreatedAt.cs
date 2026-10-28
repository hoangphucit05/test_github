using Login.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login.Migrations
{
    [DbContext(typeof(BakeryDbContext))]
    [Migration("20261021010000_AddShiftCreatedAt")]
    public partial class AddShiftCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Shifts', 'CreatedAt') IS NULL
                    ALTER TABLE dbo.Shifts ADD CreatedAt DATETIME2 NOT NULL
                        CONSTRAINT DF_Shifts_CreatedAt DEFAULT SYSDATETIME();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Shifts', 'CreatedAt') IS NOT NULL
                    ALTER TABLE dbo.Shifts DROP CONSTRAINT DF_Shifts_CreatedAt;

                IF COL_LENGTH('dbo.Shifts', 'CreatedAt') IS NOT NULL
                    ALTER TABLE dbo.Shifts DROP COLUMN CreatedAt;
            ");
        }
    }
}
