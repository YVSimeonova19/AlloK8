using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class RegisterVM
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    [EmailAddress(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
    public string? Email { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }

    [Compare(
        nameof(Password),
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsDifferentThanConfirmedErrorMessage")]
    public string? ConfirmPassword { get; set; }
}