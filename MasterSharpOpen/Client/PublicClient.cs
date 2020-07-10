using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Newtonsoft.Json;

namespace MasterSharpOpen.Client
{
    public class PublicClient
    {
        public HttpClient Client { get; }

        public PublicClient(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<CodeChallenges> GetChallenges()
        {
            var codeChallengeList = await Client.GetFromJsonAsync<List<Challenge>>("api/challenge");
            var codeChallenges = new CodeChallenges { Challenges = codeChallengeList };
            Console.WriteLine($"challenges from Server: {string.Join(", ", codeChallenges.Challenges.Select(x => x.Name))}");
            return codeChallenges;
        }

        public async Task<Videos> GetVideos()
        {
            var videosString = await Client.GetStringAsync("VideoList.json");
            return JsonConvert.DeserializeObject<Videos>(videosString);
        }

        public async Task<bool> PostChallenge(Challenge challenge)
        {
            var apiResult = await Client.PostAsJsonAsync("api/challenge", challenge);
            return apiResult.IsSuccessStatusCode;
        }

        public async Task<ConcurrentDictionary<string, Stream>> GetAssemblyStreams(IEnumerable<string> assemblyNames)
        {
            var streams = new ConcurrentDictionary<string, Stream>();
            await Task.WhenAll(
                assemblyNames.Select(async assemblyName =>
                {
                    var result = await Client.GetAsync($"/_framework/_bin/{assemblyName}.dll");
                    result.EnsureSuccessStatusCode();
                    streams.TryAdd(assemblyName, await result.Content.ReadAsStreamAsync());
                }));
            return streams;
        }
       
    }
}
