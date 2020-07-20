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
        //[Inject]
        //public CompilerService CompilerService { get; set; }
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }
        //private List<MetadataReference> References { get; set; }
        private Challenge NewChallenge { get; set; } = new Challenge();
        private List<Test> NewTests { get; set; } = new List<Test>();
        private CodeOutputModel outputModel;
        private bool addTests;
        private bool solveTest;
        private bool isSolved;
        private bool isFailed;
        private bool isCodeCompiling;
        private string apiResponse;
        private string validationText = "";
        private string validationCss;
        private string methodName;
        private string methodInputs;
        private string returnType;
        private string[] returnTypeItems = Enum.GetNames(typeof(InputType)).Select(x => x.ToLower()).ToArray();
        private string userName;
        private InputCollectionType returnCollectionType;

        private InputCollectionType[] returnCollectionTypes =
            Enum.GetValues(typeof(InputCollectionType)).Cast<InputCollectionType>().ToArray();
        
        private void AddTests()
        {
            if (!IsFormValid())
            {
                validationCss = "pageError";
                StateHasChanged();
                return;
            }
            var test = new Test { Append = "", TestAgainst = "" };
            NewTests.Add(test);
            addTests = true;
            StateHasChanged();
        }

        private void NewTest()
        {
            var test = new Test { Append = "", TestAgainst = "" };
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

            foreach (var test in NewTests)
            {
                test.Append = $"return {methodName}({test.Append});";
            }

            var returnTypeFull = GetFormReturnType();
            NewChallenge.Tests = NewTests;
            NewChallenge.Snippet = $"public static {returnTypeFull} {methodName}({methodInputs})" + "\n{\n\t//solution here\n}";
            solveTest = true;
            StateHasChanged();

        }
        private string GetFormReturnType() =>
            returnCollectionType switch
            {
                InputCollectionType.Array => $"{returnType}[]",
                InputCollectionType.List => $"List<{returnType}>",
                InputCollectionType.Generic => $"IEnumerable<{returnType}>",
                _ => returnType
            };

        private void ClearTests()
        {
            NewTests = new List<Test>();
            AddTests();
            StateHasChanged();
        }

        private void ClearForm()
        {
            NewChallenge = new Challenge();
            methodName = null;
            methodInputs = null;
            returnType = null;
            ClearTests();
            StateHasChanged();
        }
        private async Task SubmitCode()
        {
            outputModel = new CodeOutputModel();
            isCodeCompiling = true;
            StateHasChanged();
            NewChallenge.Solution = await Editor.GetValue();
            userName = AppStateService.UserName;
           
            var sw = new Stopwatch();
            sw.Start();
            
            var output = await PublicClient.SubmitChallenge(NewChallenge);
            AppStateService.UpdateCodeOutput(output);
            isSolved = output.Outputs.All(x => x.TestResult);
            foreach (var result in output.Outputs)
            {
                Console.WriteLine($"test: {result.TestIndex}, result: {result.TestResult}, output: {result.Codeout}");
            }
            sw.Stop();
            Console.WriteLine($"Unit tests took {sw.ElapsedMilliseconds}ms");
            if (isSolved)
            {
                NewChallenge.AddedBy = userName;
                NewChallenge.Description = $"<p>{NewChallenge.Description}</p>";
            }
            isFailed = !isSolved;
            isCodeCompiling = false;
            StateHasChanged();

        }
        private async Task AddChallengeToDb()
        {
            NewChallenge.Solution = await Editor.GetValue();

            var apiResult = await PublicClient.PostChallenge(NewChallenge);
            apiResponse = apiResult ? "Submission Successful!" : "Sorry, something went wrong. Submission failed";
            if (apiResult)
            {
                AppStateService.UpdateChallenges(NewChallenge);
            }
            StateHasChanged();
        }

       
        #region Form Validation

        private bool IsFormValid()
        {
            if (string.IsNullOrEmpty(NewChallenge.Name) || string.IsNullOrEmpty(NewChallenge.Description) ||
                string.IsNullOrEmpty(NewChallenge.Difficulty) || string.IsNullOrEmpty(methodName) || string.IsNullOrEmpty(returnType))
            {
                validationText =
                    "You are missing at least one required field. Please provide a Name, Description, Difficulty level, method name, and method return type.";
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

            //if (NewTests.Any(x => !x.Append.Contains("return ") || !x.Append.Contains(";")))
            //{
            //    validationText = "Tests must be in format: return MethodName(<Input>);  Please adjust your tests";
            //    return false;
            //}

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

        #region Form Enums

        public enum InputCollectionType
        {
            None, Single, Array, List, Generic
        }

        public enum InputType
        {
            Choose, Bool, Byte, Char, Decimal, Int, Long, String
        }

        #endregion
    }
}
