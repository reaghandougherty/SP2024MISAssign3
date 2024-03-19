using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data;
using TheMovieDB.Data;

namespace TheMovieDB
{
    public class Program
    {
        //Because of the 'await' async call to seed the DB, you need to change 'public static void Main' to 'public static async Task Main'
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("CloudConnection") ?? throw new InvalidOperationException("Connection string 'CloudConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() //THIS PART IS CRITICAL FOR ROLES TO WORK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //Two example policies below should you want to use policies
            builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(Constants.ManagerAndAdministrator, policy =>
					  policy.RequireRole(Constants.AdministratorsRole).RequireRole(Constants.ManagersRole));// This is an AND clause
			});

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(Constants.ManagerOrAdministrator, policy =>
					  policy.RequireRole(Constants.AdministratorsRole, Constants.ManagersRole)); // This is an OR clause
			});

			builder.Services.AddControllersWithViews();

            //OPTIONAL 3rd party authentication (Google/Microsoft) - See appsettings.json for keys
            //Add external authentication providers (if you want to) - https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-7.0&tabs=visual-studio
            var config = builder.Configuration;
            builder.Services.AddAuthentication()
               //required install of Microsoft.AspNetCore.Authentication.Google nuget package (version 6.0.24 due to .NET 6)
               //SEE the following URL on how to create the ClientId and ClientSecret - https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0#create-the-google-oauth-20-client-id-and-secret
               .AddGoogle(options =>
               {
                   options.ClientId = config["Authentication:Google:ClientId"];
                   options.ClientSecret = config["Authentication:Google:ClientSecret"];
               })
               //required install of Microsoft.AspNetCore.Authentication.MiscrosoftAccount nuget package (version 6.0.24 due to .NET 6)
               //SEE the following URL on how to create the ClientId and ClientSecret - https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-6.0
               .AddMicrosoftAccount(microsoftOptions =>
               {
                   microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
                   microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
               });
            //END OF OPTIONAL CODE


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                //this is a potential place to toggle sql db connections if you want to have a dev vs live db connection
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<ApplicationDbContext>();
				context.Database.Migrate();

				var testUserAdminPw = builder.Configuration.GetValue<string>("SeedUserAdminPW");
				var testUserManagerPw = builder.Configuration.GetValue<string>("SeedUserManagerPW");

				await SeedData.Initialize(services, testUserAdminPw, testUserManagerPw);
			}

			app.Run();
        }
    }
}