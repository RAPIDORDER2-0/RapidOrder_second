using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapidOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activated",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ActivationKey",
                table: "Users",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Users",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LangKey",
                table: "Users",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Users",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetDate",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetKey",
                table: "Users",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Places",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Places",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Places",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Places",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Places",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SetupId",
                table: "Places",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Places",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PlaceGroups",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PlaceGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PlaceGroups",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "PlaceGroups",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "PlaceGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "PlaceGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetupId",
                table: "PlaceGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlaceGroupId",
                table: "Missions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetupId",
                table: "Missions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Authority",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authority", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Authority_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Places_SetupId",
                table: "Places",
                column: "SetupId");

            migrationBuilder.CreateIndex(
                name: "IX_Places_UserId",
                table: "Places",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceGroups_SetupId",
                table: "PlaceGroups",
                column: "SetupId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_PlaceGroupId",
                table: "Missions",
                column: "PlaceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_SetupId",
                table: "Missions",
                column: "SetupId");

            migrationBuilder.CreateIndex(
                name: "IX_Authority_UserId",
                table: "Authority",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_PlaceGroups_PlaceGroupId",
                table: "Missions",
                column: "PlaceGroupId",
                principalTable: "PlaceGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Missions_Setups_SetupId",
                table: "Missions",
                column: "SetupId",
                principalTable: "Setups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceGroups_Setups_SetupId",
                table: "PlaceGroups",
                column: "SetupId",
                principalTable: "Setups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Setups_SetupId",
                table: "Places",
                column: "SetupId",
                principalTable: "Setups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Users_UserId",
                table: "Places",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Missions_PlaceGroups_PlaceGroupId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_Missions_Setups_SetupId",
                table: "Missions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaceGroups_Setups_SetupId",
                table: "PlaceGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Setups_SetupId",
                table: "Places");

            migrationBuilder.DropForeignKey(
                name: "FK_Places_Users_UserId",
                table: "Places");

            migrationBuilder.DropTable(
                name: "Authority");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Setups");

            migrationBuilder.DropIndex(
                name: "IX_Places_SetupId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_UserId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_PlaceGroups_SetupId",
                table: "PlaceGroups");

            migrationBuilder.DropIndex(
                name: "IX_Missions_PlaceGroupId",
                table: "Missions");

            migrationBuilder.DropIndex(
                name: "IX_Missions_SetupId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "Activated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ActivationKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LangKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "SetupId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "SetupId",
                table: "PlaceGroups");

            migrationBuilder.DropColumn(
                name: "PlaceGroupId",
                table: "Missions");

            migrationBuilder.DropColumn(
                name: "SetupId",
                table: "Missions");
        }
    }
}
