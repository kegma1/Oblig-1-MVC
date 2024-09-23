using Microsoft.EntityFrameworkCore;

namespace Oblig1.Data {
    public class ApplicationDbContext  : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSets (tables)
        public DbSet<User>? Users { get; set; }
        public DbSet<Blog>? Blogs { get; set; }
        public DbSet<Post>? Posts { get; set; }
    }
}