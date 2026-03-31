using System.Security.Claims;
using DemoWebAPI.Core.Entities;

namespace DemoWebAPI.Application.Interfaces
{
    public interface IJwtService
    {
        // Tạo Access Token từ thông tin User (ID, Email, Role...)
        string GenerateAccessToken(User user);

        // Tạo một chuỗi ngẫu nhiên làm Refresh Token
        string GenerateRefreshToken();

        // Giải mã một Token đã hết hạn để lấy lại thông tin User (Claims)
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}