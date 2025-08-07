using DOTNET_BASE.CORE.Attributes;

namespace DOTNET_BASE.CORE.Entities;

[Table("accounts")]
public class Account
{
    [Key]
    public long Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}