using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestDash.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedByToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedById",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AssignedById",
                table: "Clients",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_AssignedById",
                table: "Clients",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_AssignedById",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_AssignedById",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Clients");
        }
    }
}
