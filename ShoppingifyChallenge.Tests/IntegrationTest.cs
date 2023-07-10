using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingifyChallenge.Tests
{
    public class IntegrationTest
    {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Program> _factory;

        public IntegrationTest()
        {
            var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions("integrationTestDb"));
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ShoppingListContext));
                    services.Remove(descriptor);
                    services.AddScoped(_ => db);
                });
            });
            _client = _factory.CreateClient();
        }
    }
}
