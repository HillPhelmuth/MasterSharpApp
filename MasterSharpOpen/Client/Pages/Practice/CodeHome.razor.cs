using System;
using System.Threading.Tasks;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeServices;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Practice
{
    public partial class CodeHome:IDisposable
    {
        [Inject]
        public CodeEditorService CodeEditorService { get; set; }
        [Inject]
        public AppStateService AppState { get; set; }
        
        private bool isAnimate = true;
        private bool isConsoleOpen;
        private bool isMonacoOpen;
        protected string CodeOutput = "";
        protected string codeSnippet;
        protected override Task OnInitializedAsync()
        {
            CodeEditorService.OnChange += StateHasChanged;
            return base.OnInitializedAsync();
        }
        protected void HandleOutputChange(string output)
        {
            CodeOutput += $"<p>{output}</p>";
            isAnimate = true;
            StateHasChanged();
        }

        private void MonacoClose()
        {
            isMonacoOpen = !isMonacoOpen;
            StateHasChanged();
        }
        private void CloseConsole()
        {
            isConsoleOpen = !isConsoleOpen;
            StateHasChanged();
            //AppState.CloseConsole();
        }
        protected Task UpdateCodeSnippet()
        {
            CodeEditorService.UpdateSnippet(codeSnippet);
            StateHasChanged();
            
            return Task.CompletedTask;
        }

        protected void ToggleAnimation() => isAnimate = false;

        public void Dispose()
        {
            Console.WriteLine("CodeHome.razor Disposed");
            CodeEditorService.OnChange -= StateHasChanged;
        }
    }
}
