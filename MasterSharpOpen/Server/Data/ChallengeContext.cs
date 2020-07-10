using MasterSharpOpen.Shared.CodeModels;
using Microsoft.EntityFrameworkCore;

namespace MasterSharpOpen.Server.Data
{
    public class ChallengeContext : DbContext
    {
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Test> Tests { get; set; }
        public ChallengeContext(DbContextOptions<ChallengeContext> options)
            : base(options)
        {

        }
    }
}

