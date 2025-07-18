﻿using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class UserSettingsVM
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? OldPassword { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? NewPassword { get; set; }

    [Compare(
        nameof(NewPassword),
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsDifferentThanConfirmedErrorMessage")]
    public string? ConfirmPassword { get; set; }
}