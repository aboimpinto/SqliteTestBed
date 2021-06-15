using Microsoft.EntityFrameworkCore;
using SqliteTestBed.Model;

namespace SqliteTestBed.DbServices
{
    public class ChannelsContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=Channels.db");
    }
}


// Add migrations and update the database
// dotnet ef migrations add InitialCreate
// dotnet ef database update