using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TicketsAPI.DTO;
using TicketsAPI.DTO.Auth;
using TicketsAPI.Entities;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ OBTENER USUARIOS ACTIVOS
        [HttpGet("ObtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var users = await _userManager.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var result = new List<object>();

            foreach (var u in users)
            {
                var role = await GetSingleRoleAsync(u);

                result.Add(new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.IsActive,
                    Role = role
                });
            }

            return Ok(result);
        }

        // ✅ OBTENER USUARIO POR ID
        [HttpGet("ObtenerUsuario/{id}")]
        public async Task<IActionResult> ObtenerUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            var role = await GetSingleRoleAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.IsActive,
                Role = role
            });
        }

        // ✅ CREAR USUARIO
        [HttpPost("CrearUsuario")]
        public async Task<IActionResult> CrearUsuario([FromBody] CreateUserRequestDTO request)
        {
            if (await _userManager.FindByNameAsync(request.Username.Trim()) != null)
                return BadRequest("El username ya existe");

            var user = new ApplicationUser
            {
                UserName = request.Username.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                IsActive = true
            };

            var create = await _userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
                return BadRequest(create.Errors.Select(e => e.Description));

            try
            {
                await SetSingleRoleAsync(user, request.Role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Usuario creado correctamente");
        }

        // ✅ ACTUALIZAR USUARIO
        [HttpPut("ActualizarUsuario/{id}")]
        public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] UpdateUserRequestDTO request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            if (!string.IsNullOrWhiteSpace(request.Username))
                user.UserName = request.Username.Trim();

            if (request.Email != null)
                user.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            var update = await _userManager.UpdateAsync(user);
            if (!update.Succeeded)
                return BadRequest(update.Errors.Select(e => e.Description));

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                try
                {
                    await SetSingleRoleAsync(user, request.Role);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok("Usuario actualizado correctamente");
        }

        // ✅ RESET CONTRASEÑA
        [HttpPost("ResetearContrasena/{id}")]
        public async Task<IActionResult> ResetearContrasena(string id, [FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok("Contraseña reseteada correctamente");
        }

        // ✅ DESACTIVAR USUARIO (SOFT DELETE)
        [HttpDelete("DesactivarUsuario/{id}")]
        public async Task<IActionResult> DesactivarUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return Ok("Usuario desactivado correctamente");
        }

        // ✅ REACTIVAR USUARIO
        [HttpPost("ReactivarUsuario/{id}")]
        public async Task<IActionResult> ReactivarUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok("Usuario reactivado correctamente");
        }

        // -------- Helpers --------

        private async Task<string?> GetSingleRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        private async Task SetSingleRoleAsync(ApplicationUser user, string role)
        {
            role = role.Trim();

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
