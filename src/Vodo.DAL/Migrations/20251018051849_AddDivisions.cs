using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Vodo.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDivisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobObjects_SiteId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "Jobs",
                newName: "JobObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_SiteId",
                table: "Jobs",
                newName: "IX_Jobs_JobObjectId");

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Geometry = table.Column<Geometry>(type: "GEOMETRY", nullable: true)
                        .Annotation("Sqlite:Srid", 4326)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobObjects_JobObjectId",
                table: "Jobs",
                column: "JobObjectId",
                principalTable: "JobObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobObjects_JobObjectId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.RenameColumn(
                name: "JobObjectId",
                table: "Jobs",
                newName: "SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_JobObjectId",
                table: "Jobs",
                newName: "IX_Jobs_SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobObjects_SiteId",
                table: "Jobs",
                column: "SiteId",
                principalTable: "JobObjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
