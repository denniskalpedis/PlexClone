using Microsoft.EntityFrameworkCore;

namespace PlexClone.Models{
    public class PlexCloneContext:DbContext{
        public PlexCloneContext(DbContextOptions<PlexCloneContext> options):base(options){}
	    // public DbSet<User> Users { get; set; }
    }
}
