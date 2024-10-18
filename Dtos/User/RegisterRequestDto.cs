using System.ComponentModel.DataAnnotations;

namespace AuthTest.Dtos.User;

public class RegisterRequestDto
{
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;
}
