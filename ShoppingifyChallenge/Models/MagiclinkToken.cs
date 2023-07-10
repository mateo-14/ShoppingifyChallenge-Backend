namespace ShoppingifyChallenge.Models
{
    public class MagiclinkToken
    {
        public string Token { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
