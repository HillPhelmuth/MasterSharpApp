using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterSharpOpen.Shared.CodeServices
{
    public class CodeEditorService
    {
        public List<string> CodeHistory { get; set; }
        public string MonacoCode { get; set; }
        public string CodeSnippet { get; set; }
        public string CurrentOutput { get; set; }
        public event Action OnChange;
        public event Func<Task> Evaluate;
        public event Func<Task> OnSnippetChange;
        public void UpdateHistory(List<string> codeHistory)
        {
            CodeHistory = codeHistory;
            NotifyStateChanged();
        }

        
        public void UpdateCurrentOutput(string output)
        {
            CurrentOutput = output;
            NotifyStateChanged();
        }
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

        

        private void NotifyStateChanged() => OnChange?.Invoke();
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
