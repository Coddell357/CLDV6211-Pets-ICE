using Microsoft.EntityFrameworkCore;

namespace mvcPetICE.Models
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Pets> Pets { get; set; }
    }
}
