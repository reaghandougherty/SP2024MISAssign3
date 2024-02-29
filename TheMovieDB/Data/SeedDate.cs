using TheMovieDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheMovieDB.Data

{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string SeedUserAdminPW, string SeedUserManagerPW)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                //create the user
                var adminID = await EnsureUser(serviceProvider, SeedUserAdminPW, "admin@moviedb.com");
                //add user to role (and make sure role exists)
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);
                
                SeedDB(context, adminID);

                var managerID = await EnsureUser(serviceProvider, SeedUserManagerPW, "manager@moviedb.com");
                await EnsureRole(serviceProvider, managerID, Constants.ManagersRole);

                SeedDB(context, managerID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true //auto-confirm email since smtp server is not available for dev work
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role)) //This checks to see if a role exists and then creates it if it does not
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            IR = await userManager.AddToRoleAsync(user, role); //assign the user to a role. The method 'RemoveFromRoleAsync' would remove the user from the role

            return IR;
        }

        private static async Task<IdentityResult> RemoveUserFromRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role)) //This checks to see if a role exists and then creates it if it does not
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            IR = await userManager.RemoveFromRoleAsync(user, role);

            return IR;
        }

        //Example code if you need/want to seed a database to ensure certain data is always there. You could also do an empty migration and code there instead.
        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            //Below is example code to seed the database.
            if (context.Actor.Any())
            {
                return;   // DB has already been seeded
            }

            context.Actor.AddRange(
                new Actor
                {
                    Name = "Joe Dirt",
                    Age = 33
                },
               new Actor
			   {
                   Name = "Ann Dirt",
				   Age = 29
			   }
             );
            context.SaveChanges();
        }
    }
}