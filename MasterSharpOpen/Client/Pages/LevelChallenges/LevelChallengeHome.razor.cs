using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.CodeServices;
using MasterSharpOpen.Shared.UserModels;
using Microsoft.AspNetCore.Components;
using TextCopy;

namespace MasterSharpOpen.Client.Pages.LevelChallenges
{
    public partial class LevelChallengeHome
    {
        [Inject]
        public CodeEditorService CodeEditorService { get; set; }
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        public IClipboard Clipboard { get; set; }
        [Inject]
        public PublicClient PublicClient { get; set; }

        public CodeChallenges CodeChallenges { get; set; }
        public Challenge SelectedChallenge { get; set; }
        public UserAppData UserAppData { get; set; }

        private string codeSnippet;
        private bool takeChallenge = false;
        private bool isCodeCompiling;
        private bool isChallengeSucceed;
        private bool isChallengeFail;
        private bool isChallengeReady;
        private bool isEditorOpen;

        protected override async Task OnInitializedAsync()
        {
            CodeChallenges ??= await PublicClient.GetChallenges();
            UserAppData = AppStateService.UserAppData;
            foreach (var challenge in CodeChallenges.Challenges.Where(challenge => UserAppData?.ChallengeSuccessIds?.Any(x => x == challenge.ID) ?? false))
            {
                challenge.UserCompleted = true;
            }

            AppStateService.SetCodeChallenges(CodeChallenges);
            AppStateService.OnChange += UpdateUserChallenges;

            isChallengeReady = true;
        }
        protected void UpdateUserChallenges()
        {
            CodeChallenges = AppStateService.CodeChallenges;
            UserAppData = AppStateService.UserAppData;
            StateHasChanged();
        }
        protected async Task HandleChallengeChanged(Challenge challenge)
        {
            Console.WriteLine($"Challenge from handler: {challenge.Name}");
            SelectedChallenge = challenge;
            await UpdateCodeSnippet(challenge.Snippet);
            takeChallenge = false;
            StateHasChanged();
        }
        private void SolveChallenge() => takeChallenge = !takeChallenge;
        protected async Task UpdateCodeSnippet(string snippet)
        {

            CodeEditorService.UpdateSnippet(snippet);
            StateHasChanged();
            await Task.Delay(50);
            isEditorOpen = true;
            codeSnippet = snippet;
            StateHasChanged();
        }
        public async Task HandleSubmit(string code)
        {
            isCodeCompiling = true;
            StateHasChanged();
            await Submit(code);

            //await HandleCodeSubmit();
        }
        public async Task Submit(string code)
        {
            var submitChallenge = new Challenge
            {
                Solution = code,
                Tests = SelectedChallenge.Tests
            };
            var output = await PublicClient.SubmitChallenge(submitChallenge);
            AppStateService.UpdateCodeOutput(output);
            
            isChallengeSucceed = output.Outputs.All(x => x.TestResult);
            isChallengeFail = !isChallengeSucceed;
            isCodeCompiling = false;
            if (isChallengeSucceed)
            {
                SelectedChallenge.UserCompleted = true;
                UserAppData.ChallengeSuccessIds.Add(SelectedChallenge.ID);
                await PublicClient.AddSuccessfulChallenge(AppStateService.UserName, SelectedChallenge.ID);
                AppStateService.UpdateUserAppData(UserAppData);
            }
            StateHasChanged();
        }
    }
}
