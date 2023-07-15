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
            var categoryExists = await _dbContext.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == userId);
            if (!categoryExists)
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

        public async Task<ICollection<Item>> GetAllItems(int userId)
        {
            var items = await _dbContext.Items.Include(i => i.Category).Where(i => i.Category.UserId == userId && !i.Deleted).ToListAsync();
            return items;
        }

        public async Task<ICollection<Item>> GetAllItemsByCategory(int userId, int categoryId)
        {
            var items = await _dbContext.Items.Include(i => i.Category).Where(i => i.Category.UserId == userId && i.CategoryId == categoryId && !i.Deleted).ToListAsync();
            return items;
        }

        public async Task<Item?> GetItemById(int userId, int itemId)
        {
            var item = await _dbContext.Items.Include(i => i.Category).FirstOrDefaultAsync(i => i.Category.UserId == userId && i.Id == itemId && !i.Deleted);
            return item;
        }

        public async Task<Result> SoftDeleteItem(int userId, int itemId)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Category.UserId == userId && i.Id == itemId);
            if (item == null)
            {
                return Result.Fail("Item not found");
            }

            item.Deleted = true;
            await _dbContext.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
