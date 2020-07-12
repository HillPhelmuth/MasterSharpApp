using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MasterSharpOpen.Server.Data;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MasterSharpOpen.Server.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly ChallengeContext context;

        public VideosController(ChallengeContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<Videos> GetVideos()
        {
            var sections = await context.VideoSections.ToListAsync();
            var videos = await context.Videos.ToListAsync();
            foreach (var section in sections)
            {
                section.Videos = videos.Where(x => x.VideoSectionID == section.ID).ToList();
            }
            return new Videos {VideoSections = sections};
        }

        [HttpPost]
        public async Task<IActionResult> AddVideo([FromBody] Video video)
        {
            if (video.ID > 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            video.VideoSectionID = await context.VideoSections.Where(x => x.Name == "User Videos").Select(x => x.ID).FirstOrDefaultAsync();
           
            await context.Videos.AddAsync(video);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
