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

                var user = await Utils.SeedUser(db);
                string name = "Category Test";
                var result = await categoriesService.CreateCategory(user.Id, name);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(name, result.Value.Name);
                Assert.AreEqual(user.Id, result.Value.UserId);
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
                var user = await Utils.SeedUser(db);
                var categories = await Utils.SeedCategories(db);
                ICategoriesService categoriesService = new CategoriesService(db);

                var resultCategories = await categoriesService.GetAllCategories(user.Id);
                Assert.AreEqual(categories.Count, resultCategories.Count);
            }
        }

        [TestMethod]
        public async Task GetAllCategories_WhenCalledWithInvalidUserId_ShouldReturnEmptyList()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                ICategoriesService categoriesService = new CategoriesService(db);
                var resultCategories = await categoriesService.GetAllCategories(2);
                Assert.AreEqual(0, resultCategories.Count);
            }
        }
    }
}
