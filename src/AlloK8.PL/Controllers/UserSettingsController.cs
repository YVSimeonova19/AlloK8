using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.DAL;
using AlloK8.PL.Extensions;
using AlloK8.PL.Models;
using Essentials.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class UserSettingsController : Controller
{
    private readonly IUserService userService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;

    public UserSettingsController(
        IUserService userService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager)
    {
        this.userService = userService;
        this.currentUser = currentUser;
        this.userManager = userManager;
    }

    [HttpGet("/profile-settings")]
    public async Task<IActionResult> UserSettings()
    {
        var model = new UserSettingsVM();
        return this.View(model);
    }

    [HttpPost("/profile-settings")]
    public async Task<IActionResult> UserSettings(UserSettingsVM model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = await this.userManager.FindByIdAsync(this.currentUser.UserId.ToString()!);
        if (user == null)
        {
            return this.NotFound();
        }

        var result = await this.userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);

        if (result.Succeeded)
        {
            return this.RedirectToAction(nameof(this.UserSettings));
        }

        this.ModelState.AssignIdentityErrors(result.Errors);
        return this.View(model);
    }
}