using BlogMaster.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Roles
            List<ApplicationRole> roles = new List<ApplicationRole>()
            {
                new ApplicationRole(){Name = "Administrator", NormalizedName = "ADMINISTRATOR", Id = Guid.Parse("F2B1B83F-D0A8-4916-94AD-FDE172BF1923")},
                new ApplicationRole(){Name = "Editor", NormalizedName = "EDITOR", Id= Guid.Parse("8227A36B-1542-4B8E-8020-E199D8C5025E")},
                new ApplicationRole(){Name = "Writter", NormalizedName = "WRITTER", Id= Guid.Parse("96DD4365-6144-4FAD-BD98-9DFBA1274CD6")},
                new ApplicationRole(){Name = "Visitor", NormalizedName = "VISITOR", Id= Guid.Parse("9535C06C-27D7-42B0-8B10-E0202E6BF6B6")},
            };
            modelBuilder.Entity<ApplicationRole>().HasData(roles);

            // Admin
            ApplicationUser admin = new ApplicationUser()
            {
                Id = Guid.Parse("31D9BA1B-47F4-4A8A-98DE-37CA4A1ADEC5"),
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "example@example.com",
                NormalizedEmail = "EXAMPLE@EXAMPLE.COM",
                EmailConfirmed = true,
                
            };

            modelBuilder.Entity<ApplicationUser>().HasData(admin);

            // Contraseñas de usuarios
            var hasher =  new PasswordHasher<ApplicationUser>();
            admin.PasswordHash = hasher.HashPassword(admin, "adminpass");

            //Add admin role
            IdentityUserRole<Guid> adminRole = new IdentityUserRole<Guid>()
            {
                UserId = admin.Id,
                RoleId = roles.First(r => r.Name == "Administrator").Id
            };
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(adminRole);


        }
    }
}
