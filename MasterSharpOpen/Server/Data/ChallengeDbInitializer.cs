using System.IO;
using System.Linq;
using System.Reflection;
using MasterSharpOpen.Shared.CodeModels;
using Newtonsoft.Json;

namespace MasterSharpOpen.Server.Data
{
    public class ChallengeDbInitializer
    {
        public static void Initialize(ChallengeContext context)
        {
            context.Database.EnsureCreated();
            if (context.Challenges.Any())
                return;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(s => s.EndsWith("ChallengeData.json"));
            string result = "";
            using var stream = assembly.GetManifestResourceStream(resourceName);
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

            context.SaveChanges();
           
        }
    }
}
