using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyList_backend.Model
{
    public class MyListDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MyList>? MyLists { get; set; }
        public MyListDbContext(DbContextOptions options) : base(options) { }
    }
}
