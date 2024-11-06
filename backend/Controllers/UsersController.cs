﻿using backend.DTOs;
using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync(RegisterDto registerDto)
        {
            var result = await _userService.CreateUserAsync(registerDto);
            if (result.Id == null) return BadRequest(result);

            return Ok(result);
        }
    }
}