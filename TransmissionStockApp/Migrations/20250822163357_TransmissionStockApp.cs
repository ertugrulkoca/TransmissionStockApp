using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransmissionStockApp.Migrations
{
    /// <inheritdoc />
    public partial class TransmissionStockApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShelfCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionDriveTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionDriveTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleBrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleModels_VehicleBrands_VehicleBrandId",
                        column: x => x.VehicleBrandId,
                        principalTable: "VehicleBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransmissionBrandId = table.Column<int>(type: "int", nullable: false),
                    SparePartNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransmissionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransmissionNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    VehicleBrandId = table.Column<int>(type: "int", nullable: true),
                    VehicleModelId = table.Column<int>(type: "int", nullable: true),
                    TransmissionStatusId = table.Column<int>(type: "int", nullable: false),
                    TransmissionDriveTypeId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransmissionStocks_TransmissionBrands_TransmissionBrandId",
                        column: x => x.TransmissionBrandId,
                        principalTable: "TransmissionBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionStocks_TransmissionDriveTypes_TransmissionDriveTypeId",
                        column: x => x.TransmissionDriveTypeId,
                        principalTable: "TransmissionDriveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TransmissionStocks_TransmissionStatuses_TransmissionStatusId",
                        column: x => x.TransmissionStatusId,
                        principalTable: "TransmissionStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionStocks_VehicleBrands_VehicleBrandId",
                        column: x => x.VehicleBrandId,
                        principalTable: "VehicleBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionStocks_VehicleModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehicleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionStockLocations",
                columns: table => new
                {
                    TransmissionStockId = table.Column<int>(type: "int", nullable: false),
                    StockLocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionStockLocations", x => new { x.TransmissionStockId, x.StockLocationId });
                    table.CheckConstraint("CK_TSL_Quantity_NonNegative", "[Quantity] >= 0");
                    table.ForeignKey(
                        name: "FK_TransmissionStockLocations_StockLocations_StockLocationId",
                        column: x => x.StockLocationId,
                        principalTable: "StockLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmissionStockLocations_TransmissionStocks_TransmissionStockId",
                        column: x => x.TransmissionStockId,
                        principalTable: "TransmissionStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockLocations_ShelfCode",
                table: "StockLocations",
                column: "ShelfCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionBrands_Name",
                table: "TransmissionBrands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionDriveTypes_Name",
                table: "TransmissionDriveTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStatuses_Name",
                table: "TransmissionStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStockLocations_StockLocationId",
                table: "TransmissionStockLocations",
                column: "StockLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStocks_TransmissionBrandId_SparePartNo_TransmissionStatusId",
                table: "TransmissionStocks",
                columns: new[] { "TransmissionBrandId", "SparePartNo", "TransmissionStatusId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStocks_TransmissionDriveTypeId",
                table: "TransmissionStocks",
                column: "TransmissionDriveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStocks_TransmissionStatusId",
                table: "TransmissionStocks",
                column: "TransmissionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStocks_VehicleBrandId",
                table: "TransmissionStocks",
                column: "VehicleBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionStocks_VehicleModelId",
                table: "TransmissionStocks",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBrands_Name",
                table: "VehicleBrands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleBrandId_Name",
                table: "VehicleModels",
                columns: new[] { "VehicleBrandId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransmissionStockLocations");

            migrationBuilder.DropTable(
                name: "StockLocations");

            migrationBuilder.DropTable(
                name: "TransmissionStocks");

            migrationBuilder.DropTable(
                name: "TransmissionBrands");

            migrationBuilder.DropTable(
                name: "TransmissionDriveTypes");

            migrationBuilder.DropTable(
                name: "TransmissionStatuses");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "VehicleBrands");
        }
    }
}
