using System.IO;
using System.Linq;
using System.Reflection;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Newtonsoft.Json;

namespace MasterSharpOpen.Server.Data
{
    public class ChallengeDbInitializer
    {
        public static void Initialize(ChallengeContext context)
        {
            context.Database.EnsureCreated();
            if (context.Challenges.Any() && context.VideoSections.Any())
                return;

            var assembly = Assembly.GetExecutingAssembly();
            if (!context.Challenges.Any())
            {
                var resourceNameChallenges = assembly.GetManifestResourceNames()
                    .SingleOrDefault(s => s.EndsWith("ChallengeData.json"));
                string result = "";
                using var stream = assembly.GetManifestResourceStream(resourceNameChallenges);
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                var codeChallenges = JsonConvert.DeserializeObject<CodeChallenges>(result);
                foreach (var challenge in codeChallenges.Challenges)
                {
                    challenge.AddedBy = "adam holm";
                    context.Challenges.Add(challenge);
                }
            }

            if (!context.VideoSections.Any())
            {
                var resourceNameVideos = assembly.GetManifestResourceNames()
                    .SingleOrDefault(s => s.EndsWith("VideoList.json"));
                string result = "";
                using var stream = assembly.GetManifestResourceStream(resourceNameVideos);
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                var videos = JsonConvert.DeserializeObject<Videos>(result);
                foreach (var video in videos.VideoSections)
                {
                    context.VideoSections.Add(video);
                }
            }
            context.SaveChanges();

        }
    }
}
