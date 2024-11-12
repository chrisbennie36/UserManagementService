using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Api.Data;

public class Message
{
    [Key]
    public int Id {get; set;}
    public string MessageText {get; set;} = string.Empty;
    public DateTime CreatedUtc{ get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
