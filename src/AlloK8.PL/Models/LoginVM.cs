using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class LoginVM
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    public string? Email { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}