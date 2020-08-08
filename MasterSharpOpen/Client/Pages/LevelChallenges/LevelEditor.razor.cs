using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeServices;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.LevelChallenges
{
    public partial class LevelEditor
    {
        
        [Parameter]
        public string CodeSnippet { get; set; }

        [Inject]
        public CodeEditorService CodeEditorService { get; set; }
        [Parameter]
        public EventCallback<string> OnCodeSubmit { get; set; }

        private string currentCode;
        public async Task SubmitCode()
        {
            currentCode = await Editor.GetValue();
            await OnCodeSubmit.InvokeAsync(currentCode);
        }

        protected override Task OnInitializedAsync()
        {
            Editor = new MonacoEditor();
            CodeEditorService.OnSnippetChange += UpdateSnippet;
            return base.OnInitializedAsync();
        }
        protected async Task UpdateSnippet()
        {
            CodeSnippet = CodeEditorService.CodeSnippet;
            currentCode = CodeSnippet;
            await Editor.SetValue(CodeSnippet);
            Console.WriteLine("Snippet Updated");
            StateHasChanged();
        }

        #region Monaco Settings

        protected MonacoEditor Editor { get; set; }
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

        protected async Task EditorOnDidInit(MonacoEditorBase editor)
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
