using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
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

        public class ResetPasswordRequestDTO
        {
            // Puedes dejarlo vacío o incluso eliminarlo si ya no lo usarás
            public string? NewPassword { get; set; }
        }

        public class ResetPasswordResponseDTO
        {
            public string Message { get; set; } = "Contraseña reseteada correctamente";
            public string TemporaryPassword { get; set; } = "";
        }

        [HttpPost("ResetearContrasena/{id}")]
        public async Task<IActionResult> ResetearContrasena(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Usuario no encontrado");

            // 1) Generar password temporal
            var tempPassword = PasswordGenerator.Generate(12); // 12 es un buen tamaño

            // 2) Reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, tempPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // (Opcional recomendado) marcar que debe cambiar contraseña al iniciar sesión
            // (Necesitas una propiedad en tu usuario, o un claim, o un campo en tu tabla)
            // user.MustChangePassword = true;
            // await _userManager.UpdateAsync(user);

            return Ok(new ResetPasswordResponseDTO
            {
                TemporaryPassword = tempPassword
            });
        }

        public static class PasswordGenerator
        {
            // Ajusta los sets si tu política lo exige
            private const string Lower = "abcdefghijklmnopqrstuvwxyz";
            private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            private const string Digits = "0123456789";
            private const string Symbols = "!@#$%^&*()-_=+[]{};:,.?";

            public static string Generate(int length = 12)
            {
                if (length < 8) length = 8; // mínimo razonable

                // Garantiza al menos 1 de cada tipo
                var chars = new List<char>
        {
            GetRandomChar(Lower),
            GetRandomChar(Upper),
            GetRandomChar(Digits),
            GetRandomChar(Symbols)
        };

                // Relleno aleatorio
                string all = Lower + Upper + Digits + Symbols;
                while (chars.Count < length)
                    chars.Add(GetRandomChar(all));

                // Mezclar para que no quede predecible
                Shuffle(chars);

                return new string(chars.ToArray());
            }

            private static char GetRandomChar(string from)
            {
                int idx = RandomNumberGenerator.GetInt32(from.Length);
                return from[idx];
            }

            private static void Shuffle(List<char> list)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = RandomNumberGenerator.GetInt32(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
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
