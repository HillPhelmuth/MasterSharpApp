using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterSharpOpen.Shared.CodeServices
{
    public class CodeEditorService
    {
        public string MonacoCode { get; set; }
        public string CodeSnippet { get; set; }
        public string CurrentOutput { get; set; }
       
        public event Func<Task> Evaluate;
        public event Func<Task> OnSnippetChange;
       
        public void UpdateSnippet(string codeSnippet)
        {
            CodeSnippet = codeSnippet;
            NotifyNewSnippet();
            //Console.WriteLine($"Event Fired - Snippet updated to {codeSnippet}");
        }

        public void EvaluateCode(string code)
        {
            MonacoCode = code;
            NotifyEvaluate();
        }

        private async void NotifyEvaluate()
        {
            if (Evaluate != null) await Evaluate?.Invoke();
        }
        private async void NotifyNewSnippet()
        {
            if (OnSnippetChange != null) await OnSnippetChange?.Invoke();
        }
    }

   
}
