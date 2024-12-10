using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class ForgotPasswordVM
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    [EmailAddress(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
    public string? Email { get; set; }
}