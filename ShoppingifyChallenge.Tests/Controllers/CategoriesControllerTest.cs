﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;
using System.Net;
using System.Net.Http.Headers;
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
                var magiclinkToken = await scope.ServiceProvider.GetRequiredService<IAuthService>().GenerateMagiclinkToken(user.Email);
                var token = await scope.ServiceProvider.GetRequiredService<IAuthService>().LoginWithMagiclinkToken(magiclinkToken);
                var totalUserCategories = await dbContext.Categories.Where(c => c.UserId == user.Id).CountAsync();

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var response = await _client.GetAsync("/api/categories");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Category>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.AreEqual(totalUserCategories, data.Count);
            }
        }
    }
}