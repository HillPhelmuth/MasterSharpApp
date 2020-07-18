using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazor.ModalDialog;
using MasterSharpOpen.Client.ExtensionMethods;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
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
        [Inject]
        protected IModalDialogService ModalService { get; set; }
        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public PublicClient PublicClient { get; set; }
        private bool isCodeCompiling;
        private bool isAnimate = true;
        private bool isConsoleOpen;
        private bool isMonacoOpen;
        private string CodeOutput = "";
        private string codeSnippet;
        private string readlinePattern { get; set; } = "Console.ReadLine()";
        
        //protected override Task OnInitializedAsync()
        //{
        //    CodeEditorService.OnChange += StateHasChanged;
        //    return base.OnInitializedAsync();
        //}
        protected void HandleOutputChange(string output)
        {
            CodeOutput += $"<p>{output}</p>";
            isCodeCompiling = false;
            StateHasChanged();
        }
        protected async Task UpdateCodeSnippet(string snippet, bool isConsole = false)
        {
            isConsoleOpen = isConsole;
            CodeEditorService.UpdateSnippet(snippet);
            StateHasChanged();
            await Task.Delay(50);
            isMonacoOpen = true;
            codeSnippet = snippet;
            StateHasChanged();
        }

        protected void HandeSubmit(string input)
        {
            Console.WriteLine("Handle Submit");
            isCodeCompiling = true;
            StateHasChanged();
            _ = OnSubmit(input);
        }
        protected async Task OnSubmit(string codeInput)
        {
            Console.WriteLine("On Submit");
            
            string result;
            var sw = new Stopwatch();
            if (codeInput.Contains(readlinePattern))
            {
                string code = await ReplaceConsoleInput(codeInput);
                sw.Start();
                result = await PublicClient.SubmitConsole(code);

                CodeOutput += $"<p>{result}</p>";
                sw.Stop();
                Console.WriteLine($"console function: {sw.ElapsedMilliseconds}ms");
                isCodeCompiling = false;
                StateHasChanged();
                return;
            }
            if (!isConsoleOpen)
            {
                sw.Start();
                result = await PublicClient.SubmitCode(codeInput);
                CodeOutput += $"<p>{result}</p>";
                sw.Stop();
                Console.WriteLine($"console function: {sw.ElapsedMilliseconds}ms");
                isCodeCompiling = false;
                StateHasChanged();
                return;
            }
            sw.Start();
            result = await PublicClient.SubmitConsole(codeInput);
            CodeOutput += $"<p>{result}</p>";
            sw.Stop();
            Console.WriteLine($"console function: {sw.ElapsedMilliseconds}ms");
            isCodeCompiling = false;
            StateHasChanged();
            
        }
        
        private async Task<string> ReplaceConsoleInput(string codeInput)
        {
            var tempCode = codeInput;
            var inputDictionary = new Dictionary<int, DataInputFormStringField>();
            var readLineIndexes = tempCode.AllIndexesOf(readlinePattern);
            var regex = new Regex(Regex.Escape(readlinePattern));
            var inputForm = new ModalDataInputForm("User Inputs", "User console input");

            for (int i = 1; i <= readLineIndexes.Count(); i++)
            {
                string userInput = "";
                var inputField1 =
                    inputForm.AddStringField($"Input{i}", $"{readlinePattern} {i}", userInput, "The user's input.");
                inputDictionary.Add(i, inputField1);
            }

            if (await inputForm.ShowAsync(ModalService))
            {
                int j = 1;
                tempCode = regex.Replace(tempCode, m =>
                {
                    var input = inputDictionary[j].Value;
                    Console.WriteLine($"Console.ReadLine() replaced with \"{input}\"");
                    j++;
                    return $"\"{input}\"";
                });
            }

            var code = tempCode;
            return code;
        }

        public void Dispose()
        {
            Console.WriteLine("CodeHome.razor Disposed");
            //CodeEditorService.OnChange -= StateHasChanged;
        }
    }
}
