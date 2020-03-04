using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PM.UserAdmin.UI.Security
{
    public class AuthHelper
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthHelper(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<bool> IsAdmin(ClaimsPrincipal user)
        {
            return (await _authorizationService.AuthorizeAsync(user, Business.Security.GroupAuthorization.AdminPolicyName)).Succeeded;
        }

        public async Task<bool> IsHeadQuarters(ClaimsPrincipal user)
        {
            return (await _authorizationService.AuthorizeAsync(user, Business.Security.GroupAuthorization.HeadQuartersPolicyName)).Succeeded;
        }

        public async Task<bool> IsStoreManager(ClaimsPrincipal user)
        {
            return (await _authorizationService.AuthorizeAsync(user, Business.Security.GroupAuthorization.StoreManagerPolicyName)).Succeeded;
        }

        public async Task<bool> IsEmployee(ClaimsPrincipal user)
        {
            return (await _authorizationService.AuthorizeAsync(user, Business.Security.GroupAuthorization.EmployeePolicyName)).Succeeded;
        }
    }
}
