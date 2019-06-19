using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PM.Business.Email
{
    public class User
    {
        private readonly IConfiguration _configuration;

        public User(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendWelcomeEmail(PM.Entity.Models.User user, string password)
        {
            var vendorUrl = _configuration.GetValue<string>("VendorWebsite");
            var subject = "Welcome to Vandivier's Product Manager Platform";
            var body = $"Hi {user.FirstName} {user.LastName}, <br /><br /> Welcome to our product management platform, below you will find your username and password. <br /><br /><b>Username:</b> {user.EmailAddress} <br /><br /><b>Password:</b> {password} <br /><br />You can access the site <a href='{vendorUrl}'>here</a> <br /><br /> Thanks, <br /> Vandivier Management";

            Helper.Send(_configuration, subject, body, new List<string>() { user.EmailAddress });
        }
    }
}
