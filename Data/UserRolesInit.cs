using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IIS_V4.Data
{
    public class UserRolesInit
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("Admin@csy.co.uk").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Admin@csy.co.uk";
                user.Email = "Admin@csy.co.uk";
                user.EmailConfirmed = true;

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrators").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Users").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Users";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Administrators").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrators";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}

