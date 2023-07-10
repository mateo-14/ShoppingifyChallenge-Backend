namespace ShoppingifyChallenge.Models
{
    public enum ShoppingListStatus
    {
        Active,
        Completed,
        Cancelled
    }

    public class ShoppingList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ShoppingListStatus Status { get; set; } = ShoppingListStatus.Active;
        public int UserId { get; set;}
        public User User { get; set; }
        public ICollection<ItemInList> Items { get; set; }
    }
}
