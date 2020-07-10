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
using Microsoft.CodeAnalysis;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class CreateChallenge
    {
        [Inject]
        public CompilerService CompilerService { get; set; }
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }
        private List<MetadataReference> References { get; set; }
        private Challenge NewChallenge { get; set; } = new Challenge();
        private List<Test> NewTests { get; set; } = new List<Test>();
        private bool addTests;
        private bool solveTest;
        private bool isSolved;
        private bool isFailed;
        private string apiResponse;
        private string validationText = "";
        private string validationCss;
        private string nameAndInputs;
        private string returnType;
        private string userName;
        protected override Task OnInitializedAsync()
        {
            References = AppStateService.References.ToList();
            return Task.CompletedTask;
        }

        private void AddTests()
        {
            if (!IsFormValid())
            {
                validationCss = "pageError";
                StateHasChanged();
                return;
            }
            var test = new Test { Append = $"return {nameAndInputs};", TestAgainst = "" };
            NewTests.Add(test);
            addTests = true;
            StateHasChanged();
        }

        private void NewTest()
        {
            var test = new Test { Append = $"return {nameAndInputs};", TestAgainst = "" };
            NewTests.Add(test);
            StateHasChanged();
        }

        private void SubmitForm()
        {
            if (!AreTestsValid())
            {
                validationCss = "pageError";
                StateHasChanged();
                return;
            }

            NewChallenge.Tests = NewTests;
            NewChallenge.Snippet = $"public {returnType} {nameAndInputs}" + "\n{\n\t//solution here\n}";
            solveTest = true;
            StateHasChanged();

        }

        private async Task SubmitCode()
        {

            userName = AppStateService.UserName;
            var refs = AppStateService.References.ToList();
            var code = await Editor.GetValue();
            var results = new List<bool>();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var test in NewChallenge.Tests)
            {
                
                var appendCode = code + test.Append;
                var result = await CompilerService.SubmitSolution(appendCode, refs, test.TestAgainst);
                Console.WriteLine($"against: {test.TestAgainst} result: {result}");
                results.Add(result);
            }
            
            sw.Stop();
            Console.WriteLine($"Unit tests took {sw.ElapsedMilliseconds}ms");
            isSolved = results.All(x => x);
            if (isSolved)
            {
                NewChallenge.AddedBy = userName;
                NewChallenge.Description = $"<p>{NewChallenge.Description}</p>";
            }
            isFailed = !isSolved;
            StateHasChanged();

        }
        private async Task AddChallengeToDb()
        {
            NewChallenge.Solution = await Editor.GetValue();

            var apiResult = await PublicClient.PostChallenge(NewChallenge);
            apiResponse = apiResult ? "Submission Successful!" : "Sorry, something went wrong. Submission failed";
            StateHasChanged();
        }

       
        #region Form Validation

        private bool IsFormValid()
        {
            if (string.IsNullOrEmpty(NewChallenge.Name) || string.IsNullOrEmpty(NewChallenge.Description) ||
                string.IsNullOrEmpty(NewChallenge.Difficulty) || string.IsNullOrEmpty(nameAndInputs) || string.IsNullOrEmpty(returnType))
            {
                validationText =
                    "You are missing at least one required field. Please provide a Name, Description, Difficulty level, and method name, and method return type.";
                return false;
            }



            validationText = "Submission Success";
            validationCss = "pageSuccess";
            return true;
        }

        private bool AreTestsValid()
        {
            if (NewTests.Count() < 2)
            {
                validationText = "Please provide at least two tests to validate submissions.";
                return false;
            }

            if (NewTests.Any(x => !x.Append.Contains("return ") || !x.Append.Contains(";")))
            {
                validationText = "Tests must be in format: return MethodName(<Type> input);  Please adjust your tests";
                return false;
            }

            if (NewTests.Any(x => string.IsNullOrEmpty(x.TestAgainst)))
            {
                validationText = "Please provide a value to test against.";
                return false;
            }

            return true;
        }
        #endregion
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
                Value = NewChallenge.Snippet ?? "private string MyProgram() \n" +
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
