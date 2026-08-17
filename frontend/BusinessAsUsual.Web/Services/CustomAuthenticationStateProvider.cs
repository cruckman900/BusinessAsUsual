using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BusinessAsUsual.Web.Services;

/// <summary>
/// Custom authentication state provider that bridges AuthenticationService to Blazor's authentication system.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationService _authService;

    public CustomAuthenticationStateProvider(AuthenticationService authService)
    {
        _authService = authService;
        _authService.OnAuthStateChanged += NotifyAuthenticationStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = _authService.IsAuthenticated
            ? CreateIdentity(_authService.CurrentUser!)
            : new ClaimsIdentity();

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    private ClaimsIdentity CreateIdentity(UserSession user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        if (!string.IsNullOrEmpty(user.JobTitle))
        {
            claims.Add(new Claim("JobTitle", user.JobTitle));
        }

        if (!string.IsNullOrEmpty(user.Department))
        {
            claims.Add(new Claim("Department", user.Department));
        }

        return new ClaimsIdentity(claims, "CustomAuth");
    }

    private void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
