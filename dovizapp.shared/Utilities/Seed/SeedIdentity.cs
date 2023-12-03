using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dovizapp.shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace dovizapp.shared.Utilities.Seed
{
    public class SeedIdentity
    {
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            // ** ROLES !
            var roles = configuration.GetSection("SeedIdentity:Roles").GetChildren().Select(i => i.Value).ToArray();
            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName) == false)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // ** USERS !
            var users = configuration.GetSection("SeedIdentity:Users").GetChildren();
            foreach (var section in users)
            {
                var firstName = section.GetValue<string>("firstName");
                var lastName = section.GetValue<string>("lastName");
                var userName = section.GetValue<string>("userName");
                var password = section.GetValue<string>("password");
                var role = section.GetValue<string>("role");

                if (await userManager.FindByNameAsync(userName) == null)
                {
                    var user = new User() {
                        FirstName = firstName,
                        LastName = lastName,
                        UserName = userName
                    };

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }
    }
}