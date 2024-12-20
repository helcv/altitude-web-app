﻿using backend.Constants;
using backend.DTOs;
using backend.Helpers;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] string search, [FromQuery] FilterParams filterParams)
        {
            var result = await _userService.GetAllUsersAsync(search, filterParams);

            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return BadRequest("User does not exist.");

            return Ok();
        }
    }
}
