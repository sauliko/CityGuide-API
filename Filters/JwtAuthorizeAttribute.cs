using CityGuide.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

public class JwtAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
        var auth = actionContext.Request.Headers.Authorization;

        if (auth == null ||
            !string.Equals(auth.Scheme, "Bearer",
                StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(auth.Parameter))
            return false;

        var tokenService = new TokenService();
        var principal = tokenService.ValidateToken(auth.Parameter);

        if (principal == null)
            return false;

        actionContext.RequestContext.Principal = principal;

        return true;
    }
}