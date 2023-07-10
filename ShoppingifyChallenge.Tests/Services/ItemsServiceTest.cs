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
                var user = await Utils.SeedUser(db);
                var categories = await Utils.SeedCategories(db);

                IItemsService service = new ItemsService(db);
                var itemName = "Item 1";
                var result = await service.CreateItem(user.Id, categories.First().Id, itemName);

                Assert.IsTrue(result.IsSuccess);
                Assert.AreEqual(itemName, result.Value.Name);
            }
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithInvalidUserId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                var categories = await Utils.SeedCategories(db);

                IItemsService service = new ItemsService(db);
                var result = await service.CreateItem(user.Id + 1, categories.First().Id, "Item 1");

                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithInvalidCategoryId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);

                IItemsService service = new ItemsService(db);
                var result = await service.CreateItem(user.Id, 1, "Item 1");

                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task GetAllItems_ReturnsAllItems()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var notDeletedItems = (await Utils.SeedItems(db)).Where(i => !i.Deleted).ToList();

                IItemsService service = new ItemsService(db);
                var resultItems = await service.GetAllItems(user.Id);

                Assert.AreEqual(notDeletedItems.Count, resultItems.Count);
            }
        }

        [TestMethod]
        public async Task GetAllItems_WhenCalledWithInvalidUserId_ReturnsEmptyList()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var resultItems = await service.GetAllItems(user.Id + 1);

                Assert.AreEqual(0, resultItems.Count);
            }
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_ReturnsAllItemsByCategory()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                var categories = await Utils.SeedCategories(db);
                var categoryId = categories.First().Id;
                var notDeletedItems = (await Utils.SeedItems(db)).Where(i => i.CategoryId == categoryId && !i.Deleted).ToList();

                IItemsService service = new ItemsService(db);
                var resultItems = await service.GetAllItemsByCategory(user.Id, categoryId);

                Assert.AreEqual(notDeletedItems.Count, resultItems.Count);
            }
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_WhenCalledWithInvalidUserId_ReturnsEmptyList()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                var categories = await Utils.SeedCategories(db);
                var categoryId = categories.First().Id;
                await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var resultItems = await service.GetAllItemsByCategory(user.Id + 1, categoryId);

                Assert.AreEqual(0, resultItems.Count);
            }
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_WhenCalledWithInvalidCategoryId_ReturnsEmptyList()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var resultItems = await service.GetAllItemsByCategory(user.Id, 12);

                Assert.AreEqual(0, resultItems.Count);
            }
        }

        [TestMethod]
        public async Task GetItemById_ReturnsItem()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var items = await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var resultItem = await service.GetItemById(user.Id, items.First().Id);

                Assert.IsNotNull(resultItem);
            }
        }

        [TestMethod]
        public async Task GetItemById_WhenCalledWithInvalidUserId_ReturnsNull()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var items = await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var resultItem = await service.GetItemById(user.Id + 1, items.First().Id);

                Assert.IsNull(resultItem);
            }
        }

        [TestMethod]
        public async Task GetItemById_WhenCalledWithDeletedItemId_ReturnsNull()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var deletedItem = (await Utils.SeedItems(db)).First(i => i.Deleted);

                IItemsService itemsService = new ItemsService(db);
                var item = await itemsService.GetItemById(user.Id, deletedItem.Id);
                Assert.IsNull(item);
            }
        }

        [TestMethod]
        public async Task SoftDeleteItem_ReturnsSuccess()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var items = await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var result = await service.SoftDeleteItem(user.Id, items.First().Id);

                Assert.IsTrue(result.IsSuccess);
            }
        }

        [TestMethod]
        public async Task SoftDeleteItem_WhenCalledWithInvalidUserId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);
                await Utils.SeedCategories(db);
                var items = await Utils.SeedItems(db);

                IItemsService service = new ItemsService(db);
                var result = await service.SoftDeleteItem(user.Id + 1, items.First().Id);

                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task SoftDeleteItem_WhenCalledWithInvalidItemId_ReturnsError()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var user = await Utils.SeedUser(db);

                IItemsService service = new ItemsService(db);
                var result = await service.SoftDeleteItem(user.Id, 1);

                Assert.IsTrue(result.IsFailed);
            }
        }
    }
}
