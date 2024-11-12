using System.ComponentModel.DataAnnotations;
using UserManagementService.Shared.Enums;

namespace UserManagementService.Api.WebApplication.Dtos
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Client;
    }
}