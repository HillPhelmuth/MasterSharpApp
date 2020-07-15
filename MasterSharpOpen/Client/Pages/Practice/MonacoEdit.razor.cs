using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeServices;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;
using TextCopy;

namespace MasterSharpOpen.Client.Pages.Practice
{
    public partial class MonacoEdit : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime jsRuntime { get; set; }
        
        [Inject]
        public CodeEditorService CodeEditorService { get; set; }
        [Inject]
        public CompilerService CompilerService { get; set; }
        [Inject]
        protected AppStateService AppState { get; set; }
        [Inject]
        public IClipboard Clipboard { get; set; }
        [Inject]
        public PublicClient PublicClient { get; set; }
        protected bool IsCodeReady { get; set; }
        protected MonacoEditor Editor { get; set; }
        protected string ValueToSet { get; set; }
        protected IEnumerable<MetadataReference> References;
        [Parameter]
        public EventCallback<string> OnOutputChange { get; set; }
        [Parameter]
        public string CodeSnippet { get; set; }
       

        protected override Task OnInitializedAsync()
        {
            Editor = new MonacoEditor();
            References = AppState.References;
            CodeEditorService.OnChange += StateHasChanged;
            CodeEditorService.OnSnippetChange += UpdateSnippet;
            return Task.CompletedTask;
        }
        public async Task SubmitCode()
        {
            var code = await Editor.GetValue();
            var output = await PublicClient.SubmitCode(code);
            await OnOutputChange.InvokeAsync(output);
        }

        protected async Task UpdateSnippet()
        {
            CodeSnippet = CodeEditorService.CodeSnippet;
            await Editor.SetValue(CodeSnippet);
            Console.WriteLine("Snippet Updated");
            StateHasChanged();
        }
       
        protected StandaloneEditorConstructionOptions EditorOptionsRoslyn(MonacoEditor editor)
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

        public void Dispose()
        {
            Console.WriteLine("MonacoEdit.razor Disposed");
            CodeEditorService.OnChange -= StateHasChanged;
            CodeEditorService.OnSnippetChange -= UpdateSnippet;
        }
    }
}
