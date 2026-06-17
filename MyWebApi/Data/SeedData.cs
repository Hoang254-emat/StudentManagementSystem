using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MyWebApi.Data
{
    public static class SeedData
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();

            await context.Database.MigrateAsync();

            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "User" },
                    new Role { RoleName = "Teacher" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync(u => u.Id == "A001"))
            {
                using var hmac = new HMACSHA512();

                var admin = new User
                {
                    Id = "A001",
                    Username = "hoanggiaochu",
                    RoleId = 1
                };

                admin.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("admin123"));
                admin.PasswordSalt = hmac.Key;

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}