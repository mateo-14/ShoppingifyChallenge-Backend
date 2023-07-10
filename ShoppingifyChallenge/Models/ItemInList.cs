namespace ShoppingifyChallenge.Models
{
    public class ItemInList
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int ShoppingListId { get; set; }
        public Item Item { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public int Quantity { get; set; }
        public bool Checked { get; set; }
    }
}
