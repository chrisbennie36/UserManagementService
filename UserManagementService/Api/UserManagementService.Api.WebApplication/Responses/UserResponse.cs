using UserManagementService.Shared.Enums;

namespace UserManagementService.Api.WebApplication.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
