using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FundRaising.Server.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UserPasswordHashField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salt",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "users",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<string>(
                name: "salt",
                table: "users",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
