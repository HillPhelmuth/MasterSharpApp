using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterSharpOpen.Server.Data;
using MasterSharpOpen.Shared.CodeModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterSharpOpen.Server.Controllers
{
    [Route("api/challenge")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ChallengeContext context;
        
        public ChallengeController(ChallengeContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<List<Challenge>> GetChallenges()
        {
            var challenges = await context.Challenges.ToListAsync();
            var tests = await context.Tests.ToListAsync();
            foreach (var challenge in challenges)
            {
                challenge.Tests = tests.Where(x => x.ChallengeID == challenge.ID).ToList();
            }
            return challenges;
        }

        [HttpPost]
        public async Task<IActionResult> AddChallenge([FromBody] Challenge challenge)
        {
            if (challenge.ID > 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            
            await context.Challenges.AddAsync(challenge);
            foreach (var test in challenge.Tests)
            {
                test.ChallengeID = challenge.ID;
                await context.Tests.AddAsync(test);
            }
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
