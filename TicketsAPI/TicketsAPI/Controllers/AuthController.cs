using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketsAPI.DTO;
using TicketsAPI.DTO.Auth;
using TicketsAPI.Entities;
using TicketsAPI.Services;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenService _jwt;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtTokenService jwt,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
            _roleManager = roleManager;
        }

        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            // username repetido
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("El username ya existe");

            // email repetido (opcional)
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null)
                    return BadRequest("El email ya existe");
            }

            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors.Select(e => e.Description));

            // Rol por defecto (NO dejes que el frontend se invente roles si no quieres)
            var role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role;

            // Asegura que exista el rol
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return Ok(new MessageResponseDTO { Message = "Usuario registrado correctamente" });
        }

        // ✅ ME (datos del usuario logueado)
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.IsActive,
                Roles = roles
            });
        }

        // ✅ CHANGE PASSWORD
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] DTO.ChangePasswordRequestDTO request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Contraseña actualizada correctamente" });
        }

        // ✅ LOGIN (ya lo tenías, pero te lo dejo aquí consistente)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !user.IsActive)
                return Unauthorized("Credenciales inválidas");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Credenciales inválidas");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var (token, expiresAt) = _jwt.GenerateToken(user.Id, user.UserName!, role);

            return Ok(new LoginResponseDTO
            {
                Token = token,
                ExpiresAt = expiresAt,
                Role = role,
                UserId = user.Id,
                Username = user.UserName!
            });
        }
    }
}
