using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
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
            if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                return Result.Fail("User not found");
            }

            var category = new Category { Name = name, UserId = userId };
            var added = _dbContext.Categories.Add(category);
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
