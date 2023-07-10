
using Microsoft.EntityFrameworkCore;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Services;

namespace ShoppingifyChallenge.Tests.Services
{
    [TestClass]
    public class AuthServiceTest
    {
        [TestMethod]
        public async Task GenerateMagiclinkToken_ReturnsToken()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                IAuthService authService = new AuthService(db, Utils.configuration);
                string email = "example@email.com";
                var generatedToken = await authService.GenerateMagiclinkToken(email);                 

                Assert.IsNotNull(generatedToken);
                var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
                Assert.IsNotNull(user);
            }
        }

        [TestMethod]
        public async Task LoginWithMagiclinkToken_AddToBlacklist_And_ReturnsAuthJWT()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                IAuthService authService = new AuthService(db, Utils.configuration);
                string email = "example@email.com";
                var token = await authService.GenerateMagiclinkToken(email);

                var result = await authService.LoginWithMagiclinkToken(token);
                Assert.IsTrue(result.IsSuccess);

                var inBlacklist = await authService.IsMagiclinkTokenInBlacklist(token);
                Assert.IsTrue(inBlacklist);
            }
        }

        [TestMethod]
        public async Task LoginWithMagiclinkToken_ReturnsIsNotValid()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                var token = "invalidtoken";
                IAuthService authService = new AuthService(db, Utils.configuration);
                var result = await authService.LoginWithMagiclinkToken(token);
                Assert.IsTrue(result.IsFailed);
            }
        }

        [TestMethod]
        public async Task LoginWithMagiclinkToken_ReturnsIsNotValid_WhenTokenIsInBlacklist()
        {
            using (var db = new ShoppingListContext(Utils.GetInMemoryDbContextOptions()))
            {
                IAuthService authService = new AuthService(db, Utils.configuration);
                string email = "example@email.com";
                var token = await authService.GenerateMagiclinkToken(email);

                await authService.LoginWithMagiclinkToken(token); // Login and add to blacklist

                var result = await authService.LoginWithMagiclinkToken(token); // Try to login again with same token
                Assert.IsTrue(result.IsFailed);
            }
        }
    }
}
