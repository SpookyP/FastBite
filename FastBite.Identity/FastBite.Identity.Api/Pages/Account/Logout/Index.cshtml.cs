// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FastBite.Pages.Logout;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    [BindProperty] 
    public string? LogoutId { get; set; }

    public string CancelUrl { get; set; } = string.Empty;

    public Index(IIdentityServerInteractionService interaction, IEventService events, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _interaction = interaction;
        _events = events;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGet(string? logoutId)
    {
        LogoutId = logoutId;

        CancelUrl = _configuration["ClientOrigin"] ?? "";

        var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

        if (User.Identity?.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            showLogoutPrompt = false;
        }
        else
        {
            var context = await _interaction.GetLogoutContextAsync(LogoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // force Prompt
                showLogoutPrompt = true;
            }
        }
            
        if (showLogoutPrompt == false)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await OnPost();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            LogoutId ??= await _interaction.CreateLogoutContextAsync();

            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();

            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            Telemetry.Metrics.UserLogout(idp);

            if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
            {
                if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    var url = Url.Page("/Account/Logout/Loggedout", new { logoutId = LogoutId });
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }
        }

        var clientOrigin = _configuration["ClientOrigin"] ?? "";
        return Redirect(clientOrigin);
    }
}
