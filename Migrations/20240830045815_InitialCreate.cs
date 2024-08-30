using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineVehicleRentalSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Vehicles_Vehicle1Id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Vehicle1Id",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Vehicle1Id",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VehicleId1",
                table: "Bookings",
                column: "VehicleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Vehicles_VehicleId1",
                table: "Bookings",
                column: "VehicleId1",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Vehicles_VehicleId1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VehicleId1",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "Vehicle1Id",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Vehicle1Id",
                table: "Bookings",
                column: "Vehicle1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Vehicles_Vehicle1Id",
                table: "Bookings",
                column: "Vehicle1Id",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
