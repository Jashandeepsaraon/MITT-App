namespace Scheduler_App.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Scheduler_App.Models;
    using Scheduler_App.Models.Enum;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Scheduler_App.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Scheduler_App.Models.ApplicationDbContext context)
        {
            var userManager =
               new UserManager<ApplicationUser>(
                       new UserStore<ApplicationUser>(context));

            var roleManager =
               new RoleManager<IdentityRole>(
                   new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(p => p.Name == nameof(UserRoles.Admin)))
            {
                var adminRole = new IdentityRole(nameof(UserRoles.Admin));
                roleManager.Create(adminRole);
            }

            if (!context.Roles.Any(p => p.Name == nameof(UserRoles.Instructor)))
            {
                var instructorRole = new IdentityRole(nameof(UserRoles.Instructor));
                roleManager.Create(instructorRole);
            }

            //if (!userManager.IsInRole(user.Id, "Developer"))
            //{
            //    userManager.AddToRole(developerUser.Id, "Developer");
            //}

            if (!context.Roles.Any(p => p.Name == nameof(UserRoles.Student)))
            {
                var studentRole = new IdentityRole(nameof(UserRoles.Student));
                roleManager.Create(studentRole);
            }

            ApplicationUser adminUser;

            if (!context.Users.Any(p => p.UserName == "admin@schedularApp.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@schedularApp.com";
                adminUser.Email = "admin@schedularApp.com";
                adminUser.EmailConfirmed = true; //To Test Email if Confirmed or not.

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.First(p => p.UserName == "admin@schedularApp.com");
            }

            if (!userManager.IsInRole(adminUser.Id, nameof(UserRoles.Admin)))
            {
                userManager.AddToRole(adminUser.Id, nameof(UserRoles.Admin));
            }

            context.SaveChanges();
        }
    }
}
