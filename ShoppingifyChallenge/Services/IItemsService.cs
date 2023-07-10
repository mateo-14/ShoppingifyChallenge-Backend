using FluentResults;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Services
{
    public interface IItemsService
    {
        public Task<Result<Item>> CreateItem(int userId, int categoryId, string name);
        public Task<Result<ICollection<Item>>> GetAllItems(int userId);
        public Task<Result<ICollection<Item>>> GetAllItemsByCategory(int userId, int categoryId);
        public Task<Result<Item>> GetItemById(int userId, int itemId);
        public Task<Result> SoftDeleteItem(int userId, int itemId);
    }
}
