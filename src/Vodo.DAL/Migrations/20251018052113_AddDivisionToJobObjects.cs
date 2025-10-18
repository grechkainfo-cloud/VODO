using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vodo.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDivisionToJobObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "JobObjects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobObjects_DivisionId",
                table: "JobObjects",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobObjects_Divisions_DivisionId",
                table: "JobObjects",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobObjects_Divisions_DivisionId",
                table: "JobObjects");

            migrationBuilder.DropIndex(
                name: "IX_JobObjects_DivisionId",
                table: "JobObjects");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "JobObjects");
        }
    }
}
