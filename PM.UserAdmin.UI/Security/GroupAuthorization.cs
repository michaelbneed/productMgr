using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace PM.UserAdmin.UI.Security
{
    public class GroupAuthorization
    {
        public const string AdminPolicyName = "Administrators";

        public const string HeadQuartersPolicyName = "HeadQuarters";

        public const string StoreManagerPolicyName = "StoreManagers";

        public const string EmployeePolicyName = "Employees";

        public static bool AdminPolicyAssertion(AuthorizationHandlerContext context, IConfiguration configuration)
        {
            var allowedSecurityGroups = new List<string>()
            {
                configuration.GetValue<string>("SecurityGroups:Admin")
            };

            return context.User.Claims.Where(claim => claim.Type == "groups").Select(x => x).Any(x => allowedSecurityGroups.Contains(x.Value));
        }

        public static bool HeadQuartersPolicyAssertion(AuthorizationHandlerContext context, IConfiguration configuration)
        {
            var allowedSecurityGroups = new List<string>()
            {
                configuration.GetValue<string>("SecurityGroups:Admin"),
                configuration.GetValue<string>("SecurityGroups:HeadQuarters")
            };

            return context.User.Claims.Where(claim => claim.Type == "groups").Select(x => x).Any(x => allowedSecurityGroups.Contains(x.Value));
        }

        public static bool StoreManagersPolicyAssertion(AuthorizationHandlerContext context, IConfiguration configuration)
        {
            var allowedSecurityGroups = new List<string>()
            {
                configuration.GetValue<string>("SecurityGroups:Admin"),
                configuration.GetValue<string>("SecurityGroups:HeadQuarters"),
                configuration.GetValue<string>("SecurityGroups:StoreManager")
            };

            return context.User.Claims.Where(claim => claim.Type == "groups").Select(x => x).Any(x => allowedSecurityGroups.Contains(x.Value));
        }

        public static bool EmployeePolicyAssertion(AuthorizationHandlerContext context, IConfiguration configuration)
        {
            var allowedSecurityGroups = new List<string>()
            {
                configuration.GetValue<string>("SecurityGroups:Admin"),
                configuration.GetValue<string>("SecurityGroups:HeadQuarters"),
                configuration.GetValue<string>("SecurityGroups:StoreManager"),
                configuration.GetValue<string>("SecurityGroups:Employee")
            };

            return context.User.Claims.Where(claim => claim.Type == "groups").Select(x => x).Any(x => allowedSecurityGroups.Contains(x.Value));
        }
    }
}
