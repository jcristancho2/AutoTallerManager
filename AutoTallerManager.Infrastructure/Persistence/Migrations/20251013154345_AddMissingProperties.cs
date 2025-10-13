using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTallerManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "orders_details",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "orders_details",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "orders_details",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DescripcionTrabajo",
                table: "ordenes_servicio",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroFactura",
                table: "bills",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "bills",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "orders_details");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "orders_details");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "orders_details");

            migrationBuilder.DropColumn(
                name: "DescripcionTrabajo",
                table: "ordenes_servicio");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "bills");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "bills");
        }
    }
}
