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
    public class ItemsServiceTest
    {
        [TestMethod]
        public async Task CreateItem_ReturnsItem()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                await Utils.SeedDb(db);

                IItemsService service = new ItemsService(db);
                var itemName = "Item 1";
                var result = await service.CreateItem(Utils.User.Id, Utils.Categories.First().Id, itemName);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(itemName, result.Value.Name);
            }
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithInvalidUserId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                await Utils.SeedDb(db);

                IItemsService service = new ItemsService(db);
                var result = await service.CreateItem(Utils.User.Id + 1, Utils.Categories.First().Id, "Item 1");

                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithInvalidCategoryId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                await Utils.SeedDb(db);

                IItemsService service = new ItemsService(db);
                var result = await service.CreateItem(Utils.User.Id, Utils.Categories.Last().Id + 1, "Item 1");

                Assert.IsTrue(result.IsFailed);
            }
        }
    }
}
