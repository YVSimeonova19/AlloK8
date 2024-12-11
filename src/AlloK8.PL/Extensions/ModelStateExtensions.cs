using System.Collections.Generic;
using System.Linq;
using AlloK8.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AlloK8.PL.Extensions;

public static class ModelStateExtensions
{
    public static void AssignIdentityErrors(
        this ModelStateDictionary modelState,
        IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
        {
            var key = string.Empty;
            if (error.Code.StartsWith("Password"))
            {
                key = "Password";
            }

            modelState.AddModelError(key, T.ResourceManager.GetStringOrDefault($"{error.Code}ErrorMessage"));
        }
    }

    public static string? GetFirstGlobalError(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => string.IsNullOrEmpty(x.Key))
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage)
            .FirstOrDefault();
    }
}