using Microsoft.EntityFrameworkCore;

namespace MyList_backend.Model
{
    public class MyListContext : DbContext
    {
        public DbSet<User>? Users { get; set; }

        public MyListContext(DbContextOptions options) : base(options) { }
    }
}
