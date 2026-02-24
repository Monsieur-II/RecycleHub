using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecycleHub.Pg.Sdk.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Centers_With_Recycled_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecycledProducts",
                table: "RecycleCenters",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecycledProducts",
                table: "RecycleCenters");
        }
    }
}
