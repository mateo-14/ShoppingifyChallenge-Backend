using FluentResults;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Services
{
    public interface IItemsService
    {
        public Task<Result<Item>> CreateItem(int userId, int categoryId, string name);
        public Task<ICollection<Item>> GetAllItems(int userId);
        public Task<ICollection<Item>> GetAllItemsByCategory(int userId, int categoryId);
        public Task<Item?> GetItemById(int userId, int itemId);
        public Task<Result> SoftDeleteItem(int userId, int itemId);
    }
}
