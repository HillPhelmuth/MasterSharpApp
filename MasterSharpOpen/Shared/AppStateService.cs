﻿using System;
using System.Collections.Generic;
using System.Linq;
using MasterSharpOpen.Shared.CodeModels;
using MasterSharpOpen.Shared.VideoModels;
using Microsoft.CodeAnalysis;

namespace MasterSharpOpen.Shared
{
    public class AppStateService
    {
        public CodeChallenges CodeChallenges { get; private set; }
        public Videos Videos { get; private set; }
        //public IEnumerable<MetadataReference> References { get; private set; }
        public string UserName { get; private set; }
        public CodeOutputModel CodeOutput { get; private set; }
        public event Action OnChange;
        public event Action<int> OnTabChange;

        public void SetCodeChallenges(CodeChallenges codeChallenges/*, IEnumerable<MetadataReference> assemblyReferences*/)
        {
            CodeChallenges = codeChallenges;
            //References = assemblyReferences;
            NotifyStateHasChanged();
        }

        public void SetVideos(Videos videos)
        {
            Videos = videos;
            NotifyStateHasChanged();
        }
        public void UpdateChallenges(Challenge challenge)
        {
            CodeChallenges.Challenges.Add(challenge);
            NotifyStateHasChanged();
        }
        public void UpdateUserName(string name)
        {
            UserName = name;
            NotifyStateHasChanged();
        }

        public void UpdateCodeOutput(CodeOutputModel codeOutput)
        {
            foreach (var output in codeOutput.Outputs ?? new List<Output>())
            {
                output.CssClass = output.TestResult ? "testPass" : "testFail";
            }
            CodeOutput = codeOutput;
            Console.WriteLine($"Output State Updated");
            NotifyStateHasChanged();
        }

        public void UpdateTabNavigation(int tab) => OnTabChange?.Invoke(tab);
        private void NotifyStateHasChanged() => OnChange?.Invoke();
    }
}
