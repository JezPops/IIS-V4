using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class ValidUserBranch : IAuthorizationRequirement
    {
        public Int64? BranchID { get; set; }

        public ValidUserBranch(Int64? branchID)
        {
            BranchID = branchID;
        }
    }

    public static class Policies
    {
        public const string Branch = "Branch";
        public static AuthorizationPolicy BranchPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .Build();
        }
    }

    public class BranchAuthHandler : AuthorizationHandler<ValidUserBranch>
    {
        private readonly UserManager<ApplicationUser> userManager;
        public BranchAuthHandler(UserManager<ApplicationUser> _UserManager)
        {
            userManager = _UserManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidUserBranch requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.SerialNumber))
            {
                context.Fail();
            }
            else
            {
                var resource = context.Resource?.ToString();
                string BranchID = context.User.GetUserBranch(userManager).ToString();
                bool isAuthorized = (resource == BranchID) || (BranchID == "0");
                if (isAuthorized)
                { 
                    context.Succeed(requirement); 
                }
            }
            return Task.CompletedTask;
        }
    }
}


