using Microsoft.EntityFrameworkCore;

namespace Implementations.RepositoriesEF
{
    public class GameDbContext: DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options): base(options)
        { 
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerGameData> BotPlayerGameData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.Players).WithOne(e => e.Game).HasForeignKey(e => e.GameId);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<PlayerGameData>(entity => 
            {
                entity.HasKey(e => new { e.PlayerCode, e.GameId });
            });
        }

    }
}