using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingifyChallenge.Tests
{
    internal static class Utils
    {
       
        public static DbContextOptions<ShoppingListContext> GetInMemoryDbContextOptions(string? databaseName = null)
        {
            return new DbContextOptionsBuilder<ShoppingListContext>().UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString()).Options;
        } 

        public static IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "JWT:Secret", "-hLUWtBHP518FlZeA52HHD_cJk1Qd_ahabK3" },
                { "JWT:Issuer", "issuer" },
                { "JWT:Audience", "audience" },
                { "MagiclinkJWT:Secret", "ZHRMEUy2CzO24Q4Ct5FqdV2yz7vbzKe__XPQ" }
            })
            .Build();


        public static User User = new User
        {
            Email = "email@example.com"
        };

        public static ICollection<Category> Categories = new List<Category>
        {
            new Category
            {
                Name = "Category 1",
                UserId = 1
            },
            new Category
            {
                Name = "Category 2",
                UserId = 1
            },
            new Category
            {
                Name = "Category 3",
                UserId = 1
            }
        };

        public static async Task SeedDb(ShoppingListContext db)
        {
            db.Users.Add(User);
            db.Categories.AddRange(Categories);
            await db.SaveChangesAsync();
        } 
    }
}
