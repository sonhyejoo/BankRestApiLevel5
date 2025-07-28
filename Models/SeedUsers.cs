using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BankRestApi.Models;

public static class SeedUsers
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(
                   serviceProvider.GetRequiredService<
                       DbContextOptions<AppDbContext>>()))
        {
            if (context?.Users == null)
            {
                throw new ArgumentNullException("Null Users table");
            }

            if (context.Users.Any())
            {
                return;
            }

            // var password = "password";

            context.Users.AddRange(
                new User{
                    AccountName = "admin",
                    HashedPassword = "password"
                }
            );
            context.SaveChanges();
        }
    }
}