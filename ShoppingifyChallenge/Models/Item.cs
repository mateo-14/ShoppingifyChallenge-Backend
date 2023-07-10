namespace ShoppingifyChallenge.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get;set; }
        public Category Category { get; set; }
        public ICollection<ItemInList> ItemsInList { get; set; }
        public bool Deleted { get; set; }
    }
}
