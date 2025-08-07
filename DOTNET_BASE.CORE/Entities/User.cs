using DOTNET_BASE.CORE.Attributes;

namespace DOTNET_BASE.CORE.Entities;

[Table("users")]
public class User
{
    [Key]
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}