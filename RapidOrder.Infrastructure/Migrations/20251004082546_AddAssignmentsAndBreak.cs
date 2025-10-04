using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapidOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentsAndBreak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BreakStartedAt",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnBreak",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "AssignedUserId",
                table: "PlaceGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceGroups_AssignedUserId",
                table: "PlaceGroups",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceGroups_Users_AssignedUserId",
                table: "PlaceGroups",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceGroups_Users_AssignedUserId",
                table: "PlaceGroups");

            migrationBuilder.DropIndex(
                name: "IX_PlaceGroups_AssignedUserId",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "BreakStartedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsOnBreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "PlaceGroups");
        }
    }
}
