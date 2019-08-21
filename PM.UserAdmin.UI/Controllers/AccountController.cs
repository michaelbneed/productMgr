using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace PM.UserAdmin.UI.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action("Index", "RequestsAdmin", values: null, protocol: Request.Scheme);

            return SignOut( new AuthenticationProperties { RedirectUri = callbackUrl }, AzureADDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}