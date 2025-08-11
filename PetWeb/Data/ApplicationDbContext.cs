using Microsoft.EntityFrameworkCore;
using PetWeb.Models;

namespace PetWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets => Set<Pet>();
        public DbSet<AdoptionRequest> AdoptionRequests => Set<AdoptionRequest>();
    }
}