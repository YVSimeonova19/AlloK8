﻿using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class ResetPasswordVM
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }

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