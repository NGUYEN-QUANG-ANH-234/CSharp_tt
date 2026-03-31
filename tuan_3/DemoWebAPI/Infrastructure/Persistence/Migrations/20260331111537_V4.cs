using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoWebAPI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Chỉ thêm các cột còn thiếu vào bảng 'users' đã có sẵn
            // Thêm cột Role
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Thêm cột RefreshToken
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Thêm cột RefreshTokenExpiryTime
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "users",
                type: "datetime(6)",
                nullable: true);

            // Lưu ý: Nếu các bảng 'posts' và 'comments' đã có sẵn rồi 
            // thì bạn KHÔNG CẦN thêm bất kỳ lệnh nào cho chúng ở đây nữa.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa các cột đã thêm nếu muốn quay lại trạng thái cũ
            migrationBuilder.DropColumn(name: "Role", table: "users");
            migrationBuilder.DropColumn(name: "RefreshToken", table: "users");
            migrationBuilder.DropColumn(name: "RefreshTokenExpiryTime", table: "users");
        }
    }
}
