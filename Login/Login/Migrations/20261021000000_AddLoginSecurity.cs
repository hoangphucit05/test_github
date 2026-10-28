using Login.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login.Migrations
{
    [DbContext(typeof(BakeryDbContext))]
    [Migration("20261021000000_AddLoginSecurity")]
    public partial class AddLoginSecurity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Users', 'FailedLoginCount') IS NULL
                    ALTER TABLE dbo.Users ADD FailedLoginCount INT NOT NULL
                        CONSTRAINT DF_Users_FailedLoginCount DEFAULT 0;

                IF COL_LENGTH('dbo.Users', 'LockoutEnd') IS NULL
                    ALTER TABLE dbo.Users ADD LockoutEnd DATETIME2 NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Users', 'LockoutEnd') IS NOT NULL
                    ALTER TABLE dbo.Users DROP COLUMN LockoutEnd;

                IF COL_LENGTH('dbo.Users', 'FailedLoginCount') IS NOT NULL
                    ALTER TABLE dbo.Users DROP CONSTRAINT DF_Users_FailedLoginCount;

                IF COL_LENGTH('dbo.Users', 'FailedLoginCount') IS NOT NULL
                    ALTER TABLE dbo.Users DROP COLUMN FailedLoginCount;
            ");
        }
    }
}
