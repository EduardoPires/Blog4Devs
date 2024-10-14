using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blog4Devs.Models;

namespace Blog4Devs.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Blog4Devs.Models.Posts> Posts { get; set; } = default!;
        public DbSet<Blog4Devs.Models.Comments> Comment { get; set; } = default!;
    }
}
