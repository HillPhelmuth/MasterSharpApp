﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Newtonsoft.Json;

namespace MasterSharpOpen.Client
{
    public class PublicClient
    {
        private const string CHALLENGE_FUNCTION_URL = "https://challengefunction.azurewebsites.net/api";
        private const string COMPILE_FUNCTION_URL = "https://compilefunction.azurewebsites.net/api";

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
            var videos = await Client.GetFromJsonAsync<List<VideoSection>>($"{CHALLENGE_FUNCTION_URL}/videos");
            sw.Stop();
            Console.WriteLine($"videos from server: {sw.ElapsedMilliseconds}ms");
            return new Videos { VideoSections = videos };
        }

        public async Task<bool> PostChallenge(Challenge challenge)
        {
            var apiResult = await Client.PostAsJsonAsync($"{CHALLENGE_FUNCTION_URL}/challenge", challenge);
            return apiResult.IsSuccessStatusCode;
        }

        public async Task<bool> PostVideo(Video video)
        {
            var apiResult = await Client.PostAsJsonAsync($"{CHALLENGE_FUNCTION_URL}/video", video);
            return apiResult.IsSuccessStatusCode;
        }
        public async Task<CodeOutputModel> SubmitChallenge(Challenge challenge)
        {
            var sw = new Stopwatch();
            sw.Start();
            var apiResult = await Client.PostAsJsonAsync($"{COMPILE_FUNCTION_URL}/challenge", challenge);
            var result = await apiResult.Content.ReadAsStringAsync();
            sw.Stop();
            Console.WriteLine($"challenge submit too {sw.ElapsedMilliseconds}ms");
            var output = JsonConvert.DeserializeObject<CodeOutputModel>(result);
            return output;
        }

        public async Task<string> SubmitCode(string code)
        {
            var sw = new Stopwatch();
            sw.Start();
            var challenge = new Challenge { Solution = code };
            var apiResult = await Client.PostAsJsonAsync($"{COMPILE_FUNCTION_URL}/code", challenge);
            var result = await apiResult.Content.ReadAsStringAsync();
            Console.WriteLine($"code submit too {sw.ElapsedMilliseconds}ms");
            return result;
        }

        public async Task<string> SubmitConsole(string code)
        {
            var sw = new Stopwatch();
            sw.Start();
            var challenge = new Challenge { Solution = code };
            var apiResult = await Client.PostAsJsonAsync($"{COMPILE_FUNCTION_URL}/console", challenge);
            var result = await apiResult.Content.ReadAsStringAsync();
            Console.WriteLine($"code submit too {sw.ElapsedMilliseconds}ms");
            return result;
        }
    }
}
