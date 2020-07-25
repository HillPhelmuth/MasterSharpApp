using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.CodeServices;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class CreateChallenge
    {
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }
        private ChallengeForm NewChallengeForm { get; set; } = new ChallengeForm();
        private Challenge Challenge { get; set; }
        private List<Test> InputTests { get; set; } = new List<Test>();
        private bool addTests;
        private bool solveTest;
        private bool isSolved;
        private bool isFailed;
        private bool isCodeCompiling;
        private bool isSubmittedToDb;
        private string apiResponse;
        private string validationText = "";

        private void StartTests()
        {
           
            var test = new Test { Append = "", TestAgainst = "" };
            InputTests.Add(test);
            addTests = true;
            StateHasChanged();
        }

        private void NewTest()
        {
            var test = new Test { Append = "", TestAgainst = "" };
            InputTests.Add(test);
            StateHasChanged();
        }

        private void SubmitForm()
        {
            if (!AreTestsValid())
            {
                StateHasChanged();
                return;
            }
            var userName = AppStateService.UserName;
            var returnTypeFull = GetSignatureReturnType();
            Challenge = new Challenge
            {
                Name = NewChallengeForm.Name,
                Difficulty = NewChallengeForm.Difficulty,
                Description = $"<p>{NewChallengeForm.Description}</p>",
                Examples = NewChallengeForm.Examples,
                Snippet = $"public static {returnTypeFull} {NewChallengeForm.MethodName}({NewChallengeForm.MethodInputs})" + "\n{\n\t//solution here\n}",
                AddedBy = userName
            };
            SetUnitTests();
            solveTest = true;
            StateHasChanged();
        }

        private void SetUnitTests()
        {
            Challenge.Tests = InputTests.Select(test => new Test { TestAgainst = test.TestAgainst, Append = $"return {NewChallengeForm.MethodName}({test.Append});" }).ToList();
        }

        private string GetSignatureReturnType() =>
            NewChallengeForm.ReturnCollectionType switch
            {
                "Array" => $"{NewChallengeForm.ReturnType}[]",
                "List" => $"List<{NewChallengeForm.ReturnType}>",
                "Generic" => $"IEnumerable<{NewChallengeForm.ReturnType}>",
                _ => NewChallengeForm.ReturnType
            };

        private void ClearTests()
        {
            InputTests = new List<Test>();
            StartTests();
            StateHasChanged();
        }

        private void ClearForm()
        {
            NewChallengeForm = new ChallengeForm();
            ClearTests();
            StateHasChanged();
        }
        private async Task SubmitCode()
        {
            isCodeCompiling = true;
            StateHasChanged();
            Challenge.Solution = await Editor.GetValue();
            var userName = AppStateService.UserName;

            var sw = new Stopwatch();
            sw.Start();

            var output = await PublicClient.SubmitChallenge(Challenge);
            AppStateService.UpdateCodeOutput(output);
            isSolved = output.Outputs.All(x => x.TestResult);
            foreach (var result in output.Outputs)
            {
                Console.WriteLine($"test: {result.TestIndex}, result: {result.TestResult}, output: {result.Codeout}");
            }
            sw.Stop();
            Console.WriteLine($"Unit tests took {sw.ElapsedMilliseconds}ms");

            isFailed = !isSolved;
            isCodeCompiling = false;
            StateHasChanged();
        }
        private async Task AddChallengeToDb()
        {
            Challenge.Solution = await Editor.GetValue();

            var apiResult = await PublicClient.PostChallenge(Challenge);
            apiResponse = apiResult ? "Submission Successful!" : "Sorry, something went wrong. Submission failed";
            if (apiResult)
            {
                AppStateService.UpdateChallenges(Challenge);
                isSubmittedToDb = true;
            }
            StateHasChanged();
        }
        private void GoToChallenges() => AppStateService.UpdateTabNavigation(1);
        private bool AreTestsValid()
        {
            if (InputTests.Count < 2)
            {
                validationText = "Please provide at least two tests to validate submissions.";
                return false;
            }
            if (InputTests.Any(x => string.IsNullOrEmpty(x.TestAgainst)))
            {
                validationText = "Please provide a value to test against.";
                return false;
            }
            return true;
        }

        #region Monaco Editor Settings

        protected MonacoEditor Editor { get; set; }
        protected StandaloneEditorConstructionOptions EditorOptionsPuzzle(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                AutoIndent = true,
                HighlightActiveIndentGuide = true,
                Language = "csharp",
                Value = Challenge.Snippet ?? "private string MyProgram() \n" +
                    "{\n" +
                    "    string input = \"this does not\"; \n" +
                    "    string modify = input + \" suck!\"; \n" +
                    "    return modify;\n" +
                    "}\n" +
                    "return MyProgram();"
            };
        }

        protected async Task EditorOnDidInit(MonacoEditor editor)
        {
            await Editor.AddCommand((int)KeyMode.CtrlCmd | (int)KeyCode.KEY_H, (editor, keyCode) =>
            {
                Console.WriteLine("Ctrl+H : Initial editor command is triggered.");
            });
        }

        protected void OnContextMenu(EditorMouseEvent eventArg)
        {
            Console.WriteLine("OnContextMenu : " + System.Text.Json.JsonSerializer.Serialize(eventArg));
        }

        #endregion

       
    }
}
