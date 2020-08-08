using System;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.CodeModels;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class ChallengeSelect
    {
        
        
        public Challenge selectedChallenge { get; set; }
        [Parameter]
        public CodeChallenges CodeChallenges { get; set; }
        [Parameter]
        public EventCallback<Challenge> OnChallengeChanged { get; set; }

        protected Task SelectChallenge(Challenge challenge)
        {
            selectedChallenge = challenge;
            var challengeName = challenge.Name;
            
            OnChallengeChanged.InvokeAsync(selectedChallenge);
            
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
