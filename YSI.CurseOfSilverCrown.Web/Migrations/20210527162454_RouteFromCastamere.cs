﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace YSI.CurseOfSilverCrown.Web.Migrations
{
    public partial class RouteFromCastamere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "FromDomainId", "ToDomainId" },
                values: new object[] { 88, 87 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Routes",
                keyColumns: new[] { "FromDomainId", "ToDomainId" },
                keyValues: new object[] { 88, 87 });
        }
    }
}
