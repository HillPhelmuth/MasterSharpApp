using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.UserModels;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.LevelChallenges
{
    public partial class LevelSelect
    {
        public Challenge selectedChallenge { get; set; }
        [Parameter]
        public CodeChallenges CodeChallenges { get; set; }
        private List<Challenge> DisplayChallenges { get; set; } = new List<Challenge>();
        private List<UserChallengeLevel> UserChallenges { get; set; }
        [Parameter]
        public EventCallback<Challenge> OnChallengeSelected { get; set; }
        [Inject]
        public AppStateService AppStateService { get; set; }
        private UserAppData UserAppData { get; set; }
        
        private int maxLevel = 0;
        protected override Task OnInitializedAsync()
        {
            UserAppData = AppStateService.UserAppData;
            CodeChallenges = AppStateService.CodeChallenges;
            var userChallenges = CodeChallenges.Challenges
                .Where(x => UserAppData.ChallengeSuccessIds.Any(y => y == x.ID)).ToList();
            maxLevel = GetMaxLevel(userChallenges);
            var displayChallenges = maxLevel switch
            {
                6 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Hardest"),
                5 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Harder"),
                4 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Hard"),
                3 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Mid"),
                2 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Easy"),
                1 => CodeChallenges.Challenges.Where(x => x.Difficulty == "Easier"),
                _ => CodeChallenges.Challenges.Where(x => x.Difficulty == "Easiest")

            };
            DisplayChallenges = displayChallenges.ToList();
            return base.OnInitializedAsync();
        }

        private static int GetMaxLevel(IEnumerable<Challenge> userChallenges)
        {
            return userChallenges.Count(x => x.UserCompleted) / 3;
          
        }

        protected Task SelectChallenge(Challenge challenge)
        {
            selectedChallenge = challenge;
            OnChallengeSelected.InvokeAsync(selectedChallenge);
           
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
