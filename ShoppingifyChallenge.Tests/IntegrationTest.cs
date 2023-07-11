using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingifyChallenge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingifyChallenge.Tests
{
    [TestClass]
    public class IntegrationTest
    {
        protected static HttpClient _client;
        protected static WebApplicationFactory<Program> _factory;

        [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
        public static void ClassInitialize(TestContext context)
        {
            var dbGuid = Guid.NewGuid().ToString();
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ShoppingListContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ShoppingListContext>(options => options.UseInMemoryDatabase(dbGuid));
                });
            });
            _client = _factory.CreateClient();
        }

        [ClassCleanup(InheritanceBehavior.None)]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
