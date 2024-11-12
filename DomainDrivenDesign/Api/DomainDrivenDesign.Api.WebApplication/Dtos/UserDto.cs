using System.ComponentModel.DataAnnotations;
using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.WebApplication.Dtos
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