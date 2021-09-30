using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
namespace IIS_V4.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public Int64 UserBranch { get; set; }

        [PersonalData]
        public string SaveSettings { get; set; }
    }


    public static class PrincipalExtensions
    {
        public static Int64 GetUserBranch(this ClaimsPrincipal user, UserManager<ApplicationUser> userManager)
        {
            if (user.Identity.IsAuthenticated)
            {
                var appUser = userManager.FindByIdAsync(userManager.GetUserId(user)).Result;

                return appUser.UserBranch;
            }
            return 0;
        }
    }
}
