using Microsoft.EntityFrameworkCore;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingifyChallenge.Tests.Services
{
    [TestClass]
    public class CategoriesServiceTest
    {
        [TestMethod]
        public async Task CreateCategory_ShouldReturnCategory()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                ICategoriesService categoriesService = new CategoriesService(db);

                await Utils.SeedDb(db);
                string name = "Category Test";
                var result = await categoriesService.CreateCategory(Utils.User.Id, name);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(name, result.Value.Name);
                Assert.AreEqual(Utils.User.Id, result.Value.UserId);
            }
        }

        [TestMethod]
        public async Task CreateCategory_ShouldReturnError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                ICategoriesService categoriesService = new CategoriesService(db);
                string name = "Category 1";
                var result = await categoriesService.CreateCategory(2, name);
                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task GetAllCategories_ShouldReturnCategories()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                await Utils.SeedDb(db);
                var categories = await db.Categories.ToListAsync();
                ICategoriesService categoriesService = new CategoriesService(db);

                var result = await categoriesService.GetAllCategories(Utils.User.Id);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(Utils.Categories.Count, result.Value.Count - 1);
            }
        }

        [TestMethod]
        public async Task GetAllCategories_ShouldReturnError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                ICategoriesService categoriesService = new CategoriesService(db);
                var result = await categoriesService.GetAllCategories(2);
                Assert.IsTrue(result.IsFailed);
            }
        }
    }
}
