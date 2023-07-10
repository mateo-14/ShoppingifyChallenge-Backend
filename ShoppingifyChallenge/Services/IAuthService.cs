using FluentResults;

namespace ShoppingifyChallenge.Services
{
    public interface IAuthService
    {
        public Task<string> GenerateMagiclinkToken(string email);

        public Task<Result<string>> LoginWithMagiclinkToken(string token);

        public Task<bool> IsMagiclinkTokenInBlacklist(string token);
    }
}
