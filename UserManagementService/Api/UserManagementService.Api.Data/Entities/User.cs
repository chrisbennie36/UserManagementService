using UserManagementService.Api.Data.Entities;
using UserManagementService.Shared.Enums;

namespace UserManagementService.Api.Data.Entities;

public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
