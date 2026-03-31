using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Sprache;

namespace DemoWebAPI.Application.UseCases
{
    public class RefreshTokenUseCase
    {
        private readonly IJwtService _jwtService;
         private readonly IUserRepo _userRepo; 

        public RefreshTokenUseCase(IJwtService jwtService, IUserRepo userRepo)
        {
            _jwtService = jwtService;
            _userRepo = userRepo;
        }

        public async Task<TokenResponseDTO?> Execute(string expiredAccessToken, string refreshToken)
        {
            // 1. Lấy thông tin User từ Access Token đã hết hạn
            var principal = _jwtService.GetPrincipalFromExpiredToken(expiredAccessToken);
            var userIdString = principal.Identity?.Name;

            Guid userId = Guid.Parse(userIdString!);

            // 2. Tìm User trong Database (Giả lập logic tìm kiếm)
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null; // Refresh Token không hợp lệ hoặc đã hết hạn trong DB
            }

            // 3. Tạo cặp Token mới (Refresh Token Rotation - Bảo mật cao)
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // 4. Cập nhật Refresh Token mới vào DB
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
             await _userRepo.UpdateAsync(user);

            return new TokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiration = DateTime.Now.AddMinutes(15)
            };
        }
    }
}