namespace ShoppingifyChallenge.Models
{
    public enum AuthProvider
    {
        MagicLink,
        Google
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public AuthProvider AuthProvider { get; set; }
        public string? GoogleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<MagiclinkToken> MagicLinkTokens { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<ShoppingList> ShoppingLists { get; set; }
    }
}
