﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMgmt.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tasks");
        }
    }
}
