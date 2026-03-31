using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
namespace DemoWebAPI.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")] // Toàn bộ các hàm trong Controller này chỉ Admin mới vào được
	public class UserController
	{
		public UserController()
		{
		}
	}
}
