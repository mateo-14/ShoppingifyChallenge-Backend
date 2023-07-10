namespace ShoppingifyChallenge.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set;}
        public User User { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
