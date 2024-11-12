using System.ComponentModel.DataAnnotations;
using UserManagementService.Shared.Enums;

namespace UserManagementService.Api.Data;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedUtc{ get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
