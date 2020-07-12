using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Microsoft.EntityFrameworkCore;

namespace MasterSharpOpen.Server.Data
{
    public class ChallengeContext : DbContext
    {
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<VideoSection> VideoSections { get; set; }
        public DbSet<Video> Videos { get; set; }
        public ChallengeContext(DbContextOptions<ChallengeContext> options)
            : base(options)
        {

        }
    }
}

