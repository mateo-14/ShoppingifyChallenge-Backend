using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShoppingifyChallenge.Tests.Controllers
{
    [TestClass]
    public class CategoriesControllerTest : IntegrationTest
    {
        [ClassInitialize]
        public static new async Task ClassInitialize(TestContext context)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                await Utils.SeedUser(dbContext);
                await Utils.SeedCategories(dbContext);
            }
        }

        [TestMethod]
        public async Task GetCategories_ReturnsLoggedUserCategories()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var totalUserCategories = await dbContext.Categories.Where(c => c.UserId == user.Id).CountAsync();

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var response = await _client.GetAsync("/api/categories");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Category>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(totalUserCategories, data.Count);
            }
        }

        [TestMethod]
        public async Task GetCategories_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/categories");
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateCategory_ReturnsCreatedCategory()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoppingListContext>();
                var user = await dbContext.Users.FirstAsync();
                var jwt = await Utils.Login(user.Email, scope.ServiceProvider.GetRequiredService<IAuthService>());
                var totalUserCategories = await dbContext.Categories.Where(c => c.UserId == user.Id).CountAsync();

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                var newCategory = new { name = "New Category" };
                var response = await _client.PostAsync("/api/categories", new StringContent(JsonSerializer.Serialize(new { name = "New Category" }), Encoding.UTF8, "application/json"));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var createdCategory = JsonSerializer.Deserialize<Category>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.IsNotNull(createdCategory);
                Assert.AreEqual(createdCategory.Name, newCategory.name);
                Assert.AreEqual(totalUserCategories + 1, await dbContext.Categories.Where(c => c.UserId == user.Id).CountAsync());
            }
        }

        [TestMethod]
        public async Task CreateCategory_WhenCalledWithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.PostAsync("/api/categories", new StringContent(JsonSerializer.Serialize(new { name = "New Category" }), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
