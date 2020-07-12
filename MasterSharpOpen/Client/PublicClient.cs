using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Newtonsoft.Json;

namespace MasterSharpOpen.Client
{
    public class PublicClient
    {
        private const string CHALLENGE_FUNCTION_URL = "https://challengefunction.azurewebsites.net/api";
        public HttpClient Client { get; }

        public PublicClient(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<CodeChallenges> GetChallenges()
        {
            var sw = new Stopwatch();
            sw.Start();
            var codeChallengeList = await Client.GetFromJsonAsync<List<Challenge>>($"{CHALLENGE_FUNCTION_URL}/challenges");
            sw.Stop();
            Console.WriteLine($"challenges from function: {sw.ElapsedMilliseconds}ms");
            var codeChallenges = new CodeChallenges { Challenges = codeChallengeList };
            Console.WriteLine($"challenges from function: {string.Join(", ", codeChallenges.Challenges.Select(x => x.Name))}");
            return codeChallenges;
        }

        public async Task<Videos> GetVideos()
        {
            var sw = new Stopwatch();
            sw.Start();
            var videos = await Client.GetFromJsonAsync<Videos>("api/videos");
            sw.Stop();
            Console.WriteLine($"videos from server: {sw.ElapsedMilliseconds}ms");
            return videos;
        }

        public async Task<bool> PostChallenge(Challenge challenge)
        {
            var apiResult = await Client.PostAsJsonAsync($"{CHALLENGE_FUNCTION_URL}/challenge", challenge);
            return apiResult.IsSuccessStatusCode;
        }

    }
}
