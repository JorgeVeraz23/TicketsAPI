using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TicketsAPI.DTO;
using TicketsAPI.DTO.Auth;
using TicketsAPI.Entities;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // ✅ LISTAR ROLES
        [HttpGet("GetAllRoles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles
                .Select(r => new { r.Id, r.Name })
                .OrderBy(r => r.Name)
                .ToList();

            return Ok(roles);
        }

        // ✅ OBTENER ROL POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound("Rol no encontrado");

            return Ok(new { role.Id, role.Name });
        }

        // ✅ CREAR ROL
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequestDTO request)
        {
            var roleName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("El nombre del rol es obligatorio");

            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("El rol ya existe");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Rol creado correctamente" });
        }

        // ✅ RENOMBRAR ROL
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequestDTO request)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound("Rol no encontrado");

            var newName = request.Name?.Trim();
            if (string.IsNullOrWhiteSpace(newName))
                return BadRequest("El nombre del rol es obligatorio");

            // evitar duplicados por nombre
            var exists = await _roleManager.FindByNameAsync(newName);
            if (exists != null && exists.Id != role.Id)
                return BadRequest("Ya existe otro rol con ese nombre");

            role.Name = newName;

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Rol actualizado correctamente" });
        }

        // ✅ ELIMINAR ROL (bloquea si hay usuarios con ese rol)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound("Rol no encontrado");

            // Si quieres permitir borrado igual, tendrías que quitar el rol a usuarios antes.
            var usersInRole = await GetUsersInRoleSafe(role.Name!);
            if (usersInRole.Any())
                return BadRequest("No se puede eliminar: hay usuarios asignados a este rol");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Rol eliminado correctamente" });
        }

        // ✅ ASIGNAR ROL A USUARIO (por username o por userId)
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDTO request)
        {
            var roleName = request.Role?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role es obligatorio");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("El rol no existe");

            var user = await FindUser(request);
            if (user == null) return NotFound("Usuario no encontrado");

            if (!user.IsActive) return BadRequest("El usuario está inactivo");

            // Si quieres que sea 1 solo rol por usuario, primero quita los demás:
            if (request.SingleRole)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    var remove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!remove.Succeeded)
                        return BadRequest(remove.Errors.Select(e => e.Description));
                }
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
                return BadRequest("El usuario ya tiene ese rol");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Rol asignado correctamente" });
        }

        // ✅ QUITAR ROL A USUARIO
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequestDTO request)
        {
            var roleName = request.Role?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role es obligatorio");

            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("El rol no existe");

            var user = await FindUser(request);
            if (user == null) return NotFound("Usuario no encontrado");

            if (!await _userManager.IsInRoleAsync(user, roleName))
                return BadRequest("El usuario no tiene ese rol");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new MessageResponseDTO { Message = "Rol removido correctamente" });
        }

        // ✅ ROLES DE UN USUARIO
        [HttpGet("user/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Usuario no encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                user.Id,
                user.UserName,
                Roles = roles
            });
        }

        // ✅ USUARIOS DE UN ROL
        [HttpGet("{roleName}/users")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                return NotFound("El rol no existe");

            var users = await GetUsersInRoleSafe(roleName);

            return Ok(users.Select(u => new { u.Id, u.UserName, u.Email, u.IsActive }));
        }

        // ----------------- Helpers -----------------

        private async Task<ApplicationUser?> FindUser(UserLookupDTO request)
        {
            if (!string.IsNullOrWhiteSpace(request.UserId))
                return await _userManager.FindByIdAsync(request.UserId);

            if (!string.IsNullOrWhiteSpace(request.Username))
                return await _userManager.FindByNameAsync(request.Username);

            return null;
        }

        private async Task<List<ApplicationUser>> GetUsersInRoleSafe(string roleName)
        {
            // UserManager tiene GetUsersInRoleAsync, pero no siempre está disponible según store.
            // Si lo tienes, úsalo:
            try
            {
                var list = await _userManager.GetUsersInRoleAsync(roleName);
                return list.ToList();
            }
            catch
            {
                // Fallback simple: NO hay forma genérica sin consultar store.
                return new List<ApplicationUser>();
            }
        }
    }

    // ----------------- DTOs -----------------

    public class CreateRoleRequestDTO
    {
        [Required]
        public string? Name { get; set; }
    }

    public class UpdateRoleRequestDTO
    {
        [Required]
        public string? Name { get; set; }
    }

    public class UserLookupDTO
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
    }

    public class AssignRoleRequestDTO : UserLookupDTO
    {
        [Required]
        public string? Role { get; set; }

        // Si quieres forzar "un solo rol" por usuario
        public bool SingleRole { get; set; } = false;
    }

    public class RemoveRoleRequestDTO : UserLookupDTO
    {
        [Required]
        public string? Role { get; set; }
    }
}
