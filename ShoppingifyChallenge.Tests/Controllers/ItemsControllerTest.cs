using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShoppingifyChallenge.Tests.Controllers
{

    [TestClass]
    public class ItemsControllerTest : IntegrationTest
    {
        [ClassInitialize]
        public static new async Task ClassInitialize(TestContext context)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                await Utils.SeedUser(dbContext);
                await Utils.SeedCategories(dbContext);
                await Utils.SeedItems(dbContext);
            }
        }


        [TestMethod]
        public async Task CreateItem_ReturnsCreatedItem()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var totalUserItems = await dbContext.Items.Where(c => c.Category.UserId == user.Id).CountAsync();

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.PostAsync("/api/items", new StringContent(JsonSerializer.Serialize(new Item
                {
                    Name = "Test Item",
                    CategoryId = (await dbContext.Categories.FirstAsync()).Id
                }), Encoding.UTF8, "application/json"));
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Item>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(totalUserItems + 1, data.Id);
                Assert.AreEqual("Test Item", data.Name);
            }
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.PostAsync("/api/items", new StringContent(JsonSerializer.Serialize(new Item
            {
                Name = "Test Item",
                CategoryId = 1
            }), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateItem_WhenCalledWithInvalidCategory_ReturnsBadRequest()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.PostAsync("/api/items", new StringContent(JsonSerializer.Serialize(new Item
                {
                    Name = "Test Item",
                    CategoryId = 999
                }), Encoding.UTF8, "application/json"));
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        public async Task GetItems_ReturnsLoggedUserItems()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var totalUserItems = await dbContext.Items.Where(c => c.Category.UserId == user.Id).CountAsync();

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync("/api/items");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Item>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(totalUserItems, data.Count);
            }
        }

        [TestMethod]
        public async Task GetItems_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/items");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_ReturnsLoggedUserItemsByCategory()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var totalUserItems = await dbContext.Items.Where(c => c.Category.UserId == user.Id && c.CategoryId == 1).CountAsync();
                var categoryId = (await dbContext.Categories.FirstAsync(c => c.UserId == user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items?categoryId={categoryId}");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Item>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(totalUserItems, data.Count);
            }
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/items?categoryId=1");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_WhenCalledWithOtherUserCategory_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var categoryId = (await dbContext.Categories.FirstAsync(c => c.UserId != user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items?categoryId={categoryId}");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetAllItemsByCategory_WhenCalledWithInvalidCategory_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items?categoryId=999");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetItemById_ReturnsItem()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var itemId = (await dbContext.Items.FirstAsync(c => c.Category.UserId == user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items/{itemId}");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<Item>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(itemId, data.Id);
            }
        }

        [TestMethod]
        public async Task GetItemById_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/items/1");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task GetItemById_WhenCalledWithOtherUserItem_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var itemId = (await dbContext.Items.FirstAsync(c => c.Category.UserId != user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items/{itemId}");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetItemById_WhenCalledWithInvalidItem_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync($"/api/items/999");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task DeleteItem_ReturnsNoContent()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var itemId = (await dbContext.Items.FirstAsync(c => c.Category.UserId == user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.DeleteAsync($"/api/items/{itemId}");
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task DeleteItem_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/items/1");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteItem_WhenCalledWithOtherUserItem_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var itemId = (await dbContext.Items.FirstAsync(c => c.Category.UserId != user.Id)).Id;

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.DeleteAsync($"/api/items/{itemId}");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task DeleteItem_WhenCalledWithInvalidItem_ReturnsNotFound()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.DeleteAsync($"/api/items/999");
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
