﻿using backend.DTOs;
using backend.Extensions;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProfileController(IUserService userService, IHttpContextAccessor contextAccessor)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfileAsync()
        {
            var currentUserId = (_contextAccessor.HttpContext.User).GetUserId();

            var result = await _userService.GetProfileAsync(currentUserId);
            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPut("details")]
        public async Task<IActionResult> UpdateUserDetailsAsync([FromForm] UpdateUserDto updateUserDto)
        {
            var currentUserId = (_contextAccessor.HttpContext.User).GetUserId();

            var result = await _userService.UpdateUserDetailsAsync(currentUserId, updateUserDto);
            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            var currentUserId = (_contextAccessor.HttpContext.User).GetUserId();

            var result = await _userService.UpdateUserPasswordAsync(currentUserId, updatePasswordDto);
            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
