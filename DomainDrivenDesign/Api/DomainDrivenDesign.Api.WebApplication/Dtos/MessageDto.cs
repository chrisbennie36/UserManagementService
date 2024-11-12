using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Api.WebApplication.Dtos;

public class MessageDto
{
    [Required]
    public string MessageText { get; set; } = string.Empty;
}
