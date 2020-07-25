using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeModels;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using VideoModels = MasterSharpOpen.Shared.VideoModels;

namespace MasterSharpOpen.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }
        private int tabIndex = 0;
        private bool isPageReady;

        protected override async Task OnInitializedAsync()
        {
            //var codeChallenges = await PublicClient.GetChallenges();
            //AppStateService.SetCodeChallenges(codeChallenges);
            AppStateService.OnTabChange += HandleTabNavigation;
            isPageReady = true;
            StateHasChanged();
        }

        protected void HandleTabNavigation(int tab)
        {
            if (tab < 0 || tab > 4)
                return;
            tabIndex = tab;
            StateHasChanged();
        }
    }
}
