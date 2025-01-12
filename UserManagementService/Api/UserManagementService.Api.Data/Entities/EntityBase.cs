using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Api.Data.Entities;

public class EntityBase
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedUtc{ get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
