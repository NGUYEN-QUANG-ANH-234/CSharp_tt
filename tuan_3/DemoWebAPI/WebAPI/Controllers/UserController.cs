using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
namespace DemoWebAPI.WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Toàn bộ các hàm trong Controller này chỉ Admin mới vào được
	public class UserController
	{
		public UserController()
		{
		}
	}
}
