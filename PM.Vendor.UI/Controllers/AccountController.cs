﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace PM.UserAdmin.UI.Controllers
{
    public class AccountController : Controller
    {
		private readonly IOptionsMonitor<AzureADB2COptions> _options;

		[HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action("Index", "Requests", values: null, protocol: Request.Scheme);

            return SignOut( new AuthenticationProperties { RedirectUri = callbackUrl }, AzureADB2CDefaults.AuthenticationScheme);
        }

		public IActionResult AccessDenied()
        {
            return View();
        }
    }
}