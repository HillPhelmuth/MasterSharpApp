using System;
using System.Threading.Tasks;
using MasterSharpOpen.Shared.CodeModels;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class ChallengeSelect
    {
        private string description;
        private string examples;
        private bool isChallengeSelected;
        public MasterSharpOpen.Shared.CodeModels.Challenge selectedChallenge { get; set; }
        [Parameter]
        public CodeChallenges CodeChallenges { get; set; }
        [Parameter]
        public EventCallback<MasterSharpOpen.Shared.CodeModels.Challenge> OnChallengeChanged { get; set; }
        


        protected Task SelectChallenge(MasterSharpOpen.Shared.CodeModels.Challenge challenge)
        {
            selectedChallenge = challenge;
            var challengeName = challenge.Name;
            description = selectedChallenge.Description;
            examples = selectedChallenge.Examples;
            isChallengeSelected = true;
            OnChallengeChanged.InvokeAsync(selectedChallenge);
            Console.WriteLine($"Puzzle selected: {selectedChallenge.Name}");
            StateHasChanged();
            return Task.CompletedTask;

        }
    }
}
