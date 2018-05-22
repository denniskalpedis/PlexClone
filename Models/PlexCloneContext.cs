using Microsoft.EntityFrameworkCore;

namespace PlexClone.Models{
    public class PlexCloneContext:DbContext{
        public PlexCloneContext(DbContextOptions<PlexCloneContext> options):base(options){}
	    // public DbSet<User> Users { get; set; }

        public DbSet<Libraries> Libraries { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<Movies> Movies { get; set; }

    }
}
