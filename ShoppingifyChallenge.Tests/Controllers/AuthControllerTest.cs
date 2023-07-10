using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Models.Requests;
using ShoppingifyChallenge.Models.Responses;
using ShoppingifyChallenge.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ShoppingifyChallenge.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTest : IntegrationTest
    {
        [TestMethod]
        public async Task GenerateMagiclink_ReturnsToken()
        {
            var content = new StringContent(JsonSerializer.Serialize(new MagicLinkRequest { Email = "example@mail.com" }), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/magiclink", content);
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task LoginWithMagiclinkToken_ReturnsJWT()
        { 
            using (var scope = _factory.Services.CreateScope())
            {
                var magiclinkToken = await scope.ServiceProvider.GetRequiredService<IAuthService>().GenerateMagiclinkToken("example@mail.com");
                
                var response = await _client.GetAsync($"/api/auth/magiclink/login/{magiclinkToken}");       
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsNotNull(data);
                Assert.IsNotNull(data.Token);
            }         
        }
    }
}
