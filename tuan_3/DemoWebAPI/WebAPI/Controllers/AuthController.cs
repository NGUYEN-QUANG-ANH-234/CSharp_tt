using Asp.Versioning;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Infrastructure.Data; // Giả sử đây là DbContext của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly MyDbContext _context; // Thay bằng DbContext thật của bạn

        public AuthController(IJwtService jwtService, MyDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            // 1. Kiểm tra User trong DB
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == login.Email);

            // 2. Kiểm tra mật khẩu (Đây là ví dụ, thực tế bạn phải dùng BCrypt/Hash)
            if (user == null || user.Password != login.Password)
                return Unauthorized("Sai email hoặc mật khẩu");

            // 3. Tạo cặp Token
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // 4. Lưu Refresh Token vào DB
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();

            Console.WriteLine("=== DEBUG USER ROLE FROM DB ===");
            Console.WriteLine($"UserId: {user.Id}");
            Console.WriteLine($"Role (int): {(int)user.Role}");
            Console.WriteLine($"Role (enum): {user.Role}");
            Console.WriteLine($"Role.ToString(): {user.Role.ToString()}");

            return Ok(new TokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });            
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] TokenResponseDTO tokenDto)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var email = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid refresh token");

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new TokenResponseDTO { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }
    }
}