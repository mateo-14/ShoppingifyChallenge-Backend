using FluentResults;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Services
{
    public interface ICategoriesService
    {
        public Task<Result<Category>> CreateCategory(int userId, string name);
        public Task<Result<ICollection<Category>>> GetAllCategories(int userId);
    }
}
