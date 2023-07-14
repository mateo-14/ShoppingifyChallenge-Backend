using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;
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

        public static async Task<User> SeedUser(ShoppingListContext db)
        {
            var user = new User
            {
                Email = "email@example.com"
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            return user;
        }

        public static async Task<ICollection<Category>> SeedCategories(ShoppingListContext db)
        {
            var categories = new List<Category>
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

            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();

            return categories;
        }

        public static async Task<ICollection<Item>> SeedItems(ShoppingListContext db)
        {
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Item 1",
                    CategoryId = 1
                },
                new Item
                {
                    Name = "Item 2",
                    CategoryId = 1
                },
                new Item
                {
                    Name = "Deleted Item 1",
                    CategoryId = 1,
                    Deleted = true
                },
                new Item
                {
                    Name = "Item 3",
                    CategoryId = 2
                },
                new Item
                {
                    Name = "Deleted Item 2",
                    CategoryId = 3,
                    Deleted = true
                }
            };
            db.Items.AddRange(items);
            await db.SaveChangesAsync();

            return items;
        }

        public static async Task<string> Login(string email, IAuthService authService)
        {
            var magiclinkToken = await authService.GenerateMagiclinkToken(email);
            var token = await authService.LoginWithMagiclinkToken(magiclinkToken);

            return token.Value;
        }
    }
}
