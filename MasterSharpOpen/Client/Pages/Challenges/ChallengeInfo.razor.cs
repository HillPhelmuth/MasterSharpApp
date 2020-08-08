using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MasterSharpOpen.Client.Pages.Challenges
{
    public partial class ChallengeInfo
    {
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public string Examples { get; set; }
        [Parameter]
        public bool IsUserComplete { get; set; }

        private string examples = "";
        private string description = "";
        protected override Task OnInitializedAsync()
        {
            var exampleList = Examples.Split("::");
            examples = string.Join(' ', exampleList.Select(x => $"<p>{x}</p>"));
            return base.OnInitializedAsync();
        }
    }
}
