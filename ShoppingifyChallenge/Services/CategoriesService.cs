using FluentResults;
using Microsoft.EntityFrameworkCore;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ShoppingListContext _dbContext;
        public CategoriesService(ShoppingListContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Category>> CreateCategory(int userId, string name)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return Result.Fail("User not found");
            }

            var category = new Category { Name = name, UserId = userId };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return Result.Ok(category);
        }

        public async Task<ICollection<Category>> GetAllCategories(int userId)
        {
            var categories = await _dbContext.Categories.Where(c => c.UserId == userId).ToListAsync();
            return categories;
        }
    }
}
