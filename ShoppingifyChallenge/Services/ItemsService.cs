using FluentResults;
using Microsoft.EntityFrameworkCore;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Services
{
    public class ItemsService : IItemsService
    {
        private readonly ShoppingListContext _dbContext;
        public ItemsService(ShoppingListContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<Item>> CreateItem(int userId, int categoryId, string name)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null || category.UserId != userId)
            {
                return Result.Fail("Category not found");
            }

            var item = new Item
            {
                CategoryId = categoryId,
                Name = name
            };

            _dbContext.Items.Add(item);
            await _dbContext.SaveChangesAsync();
            return Result.Ok(item);
        }

        public Task<Result<ICollection<Item>>> GetAllItems(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ICollection<Item>>> GetAllItemsByCategory(int userId, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Item>> GetItemById(int userId, int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> SoftDeleteItem(int userId, int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
