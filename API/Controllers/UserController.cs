using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;

        public UserController(AppDbContext context, UserManager<User> userManager, TokenService tokenService, IMapper mapper, ImageService imageService)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
            _mapper = mapper;
            _imageService = imageService;

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            System.Console.WriteLine("\n\n\n");
            System.Console.WriteLine(loginDto.UserName);
            System.Console.WriteLine(loginDto.Password);
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized();
            }
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Roles = roles
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            System.Console.WriteLine(registerDto.UserName);
            System.Console.WriteLine(registerDto.Password);
            System.Console.WriteLine(registerDto.Email);
            var user = new User { UserName = registerDto.UserName, Email = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }
            await _userManager.AddToRoleAsync(user, "Member");
            return StatusCode(201);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<GetLoggedInUserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);

            return new GetLoggedInUserDto
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateToken(user),
                IsPremium = user.IsPremium,
                Roles = roles
            };

        }
        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> EditUser(EditUserDto userDto)
        {
            var userId = User.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;

            var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == userId);
            if (user.Id != userId && !await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Moderator"))
            {
                return Unauthorized();
            }

            if (userDto.File != null)
            {
                var maxSize = 1 * 1024 * 1024;
                if (userDto.File.Length > maxSize)
                {
                    return BadRequest(new ProblemDetails { Title = "File size exceeds the limit of 5MB." });
                }
                var imageResult = await _imageService.AddImageAsync(userDto.File);

                if (imageResult.Error != null) return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });

                user.ImageUrl = imageResult.SecureUrl.ToString();
                user.PublicId = imageResult.PublicId;
            }
            user.Description = userDto.Description;
            _context.Update(user);
            await _context.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<GetUserDto>(user);
            return Ok(updatedUserDto);
        }

    }
}