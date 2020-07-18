using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.CodeServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using TextCopy;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class CodeChallengeHome : ComponentBase, IDisposable
    {
        [Inject]
        public CodeEditorService CodeEditorService { get; set; }
        //[Inject]
        //public CompilerService CompilerService { get; set; }
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        public IClipboard Clipboard { get; set; }
        [Inject]
        public PublicClient PublicClient { get; set; }

        public CodeChallenges CodeChallenges { get; set; }
        public Challenge selectedChallenge { get; set; }

        protected string CodeSnippet;
        bool takeChallenge = false;
        private bool isCodeCompiling;
        protected bool isChallengeSucceed;
        protected bool isChallengeFail;
        protected bool isChallengeReady;
        //protected IEnumerable<MetadataReference> References;
        [Parameter] 
        public EventCallback<int> OnNotReady { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //References = AppStateService.References;
            //Console.WriteLine($"refs: {References}");
            CodeChallenges = AppStateService.CodeChallenges;
            AppStateService.OnChange += StateHasChanged;
            if ((CodeChallenges?.Challenges) == null)
            {
                await OnNotReady.InvokeAsync(0);
                return;
            }
            isChallengeReady = true;
           
        }

        private void SolveChallenge() => takeChallenge = !takeChallenge;

        public async Task SubmitCode()
        {
            isCodeCompiling = true;
            StateHasChanged();
            await Task.Run(() =>
            {
                _ = HandleCodeSubmit();
            });
            //await HandleCodeSubmit();
        }
        public async Task HandleCodeSubmit()
        {
            var code = await Editor.GetValue();
            
            var submitChallenge = new Challenge
            {
                Solution = code,
                Tests = selectedChallenge.Tests
            };
            var output = await PublicClient.SubmitChallenge(submitChallenge);
            foreach (var result in output.Outputs)
            {
                Console.WriteLine($"test: {result.TestIndex}, result: {result.TestResult}, output: {result.Codeout}");
            }
            isChallengeSucceed = output.Outputs.All(x => x.TestResult);
            isChallengeFail = !isChallengeSucceed;
            isCodeCompiling = false;
            StateHasChanged();

        }

        protected async Task HandleChallengeChanged(MasterSharpOpen.Shared.CodeModels.Challenge challenge)
        {
            Console.WriteLine($"Challenge from handler: {challenge.Name}");
            selectedChallenge = challenge;
            await UpdateSnippet();
            takeChallenge = false;
            StateHasChanged();
        }
        protected Task UpdateSnippet()
        {
            var challenge = selectedChallenge;
            CodeSnippet = challenge.Snippet;
            CodeEditorService.UpdateSnippet(CodeSnippet);
            Editor.SetValue(CodeSnippet);
            return Task.CompletedTask;
        }

        protected Task ShowAnswer()
        {
            CodeSnippet = selectedChallenge.Solution;
            Editor.SetValue(CodeSnippet);
            CodeEditorService.UpdateSnippet(CodeSnippet);
            return Task.CompletedTask;
        }

        protected void NewChallenge()
        {
            isChallengeFail = false;
            isChallengeSucceed = false;
            takeChallenge = false;
            StateHasChanged();
        }
        public async Task CopyCodeToClipboard()
        {
            var snippetClip = await Editor.GetValue();
            await Clipboard.SetTextAsync(snippetClip);
        }

        public async Task ReadCodeFromClipboard()
        {
            var content = await Clipboard.GetTextAsync();
            await Editor.SetValue(content);
            StateHasChanged();
        }
        // Monaco Editor Settings
        protected MonacoEditor Editor { get; set; }
        protected StandaloneEditorConstructionOptions EditorOptionsPuzzle(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                AutoIndent = true,
                HighlightActiveIndentGuide = true,
                Language = "csharp",
                Value = CodeSnippet ?? "private string MyProgram() \n" +
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

        public void Dispose()
        {
            Console.WriteLine("CodeChallengeHome.razor Disposed");
            AppStateService.OnChange -= StateHasChanged;
        }
    }
}
