using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.VideoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MasterSharpOpen.Server.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        [HttpGet]
        public Task<Videos> GetVideos()
        {
            var sw = new Stopwatch();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(s => s.EndsWith("VideoList1.json"));
            string result = "";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            var videos = JsonConvert.DeserializeObject<Videos>(result);
            sw.Stop();
            Console.WriteLine($"videos from server: {sw.ElapsedMilliseconds}ms");
            return Task.FromResult(videos);
        }
    }
}
