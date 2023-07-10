using Microsoft.EntityFrameworkCore;
using ShoppingifyChallenge.Models;

namespace ShoppingifyChallenge.Data
{
    public class ShoppingListContext : DbContext
    {
        public ShoppingListContext(DbContextOptions<ShoppingListContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MagiclinkToken> MagiclinkTokenBlacklist { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ItemInList> ItemsInList { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<AuthProvider>();
            modelBuilder.HasPostgresEnum<ShoppingListStatus>();

            // User
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()");
            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint("ck_users_google_notnull_id", @"""AuthProvider"" <> 'google' OR ""GoogleId"" IS NOT NULL"));

            // MagicLink
            modelBuilder.Entity<MagiclinkToken>()
                .HasKey(ml => ml.Token);

            modelBuilder.Entity<MagiclinkToken>()
                .Property(ml => ml.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<MagiclinkToken>()
                .HasOne(ml => ml.User)
                .WithMany(u => u.MagicLinkTokens)
                .HasForeignKey(ml => ml.UserId);

            // Category
            modelBuilder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            // Items
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId);

            // 
            modelBuilder.Entity<ShoppingList>()
                .Property(sl => sl.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<User>()
                .HasMany(u => u.ShoppingLists)
                .WithOne(sl => sl.User)
                .HasForeignKey(sl => sl.UserId);

            // ItemInList
            modelBuilder.Entity<ShoppingList>()
                .HasMany(sl => sl.Items)
                .WithOne(ili => ili.ShoppingList)
                .HasForeignKey(ili => ili.ShoppingListId);

            modelBuilder.Entity<ItemInList>()
                .HasOne(ili => ili.Item)
                .WithMany(i => i.ItemsInList)
                .HasForeignKey(ili => ili.ItemId);
        }
    }
}
