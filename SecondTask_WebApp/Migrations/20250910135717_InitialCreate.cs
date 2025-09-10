using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondTask_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileEntityId = table.Column<int>(type: "int", nullable: false),
                    ClassCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountClasses_Files_FileEntityId",
                        column: x => x.FileEntityId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountClassId = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSummary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountClasses_AccountClassId",
                        column: x => x.AccountClassId,
                        principalTable: "AccountClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    OpeningDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OpeningCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TurnoverDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TurnoverCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClosingDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClosingCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Balances_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountClasses_FileEntityId",
                table: "AccountClasses",
                column: "FileEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountClassId",
                table: "Accounts",
                column: "AccountClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Balances_AccountId",
                table: "Balances",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Balances");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "AccountClasses");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
