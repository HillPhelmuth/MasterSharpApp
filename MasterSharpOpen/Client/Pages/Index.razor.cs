﻿using System;
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

namespace MasterSharpOpen.Client.Pages
{
    public partial class Index
    {
        [Inject]
        public AppStateService AppStateService { get; set; }
        [Inject]
        protected PublicClient PublicClient { get; set; }

        private IEnumerable<PortableExecutableReference> references { get; set; }
        private int tabIndex = 0;
        private bool isPageReady;
       
        protected override async Task OnInitializedAsync()
        {
            var codeChallenges = await PublicClient.GetChallenges();
            Console.WriteLine($"challenges from Server: {string.Join(", ", codeChallenges.Challenges.Select(x => x.Name))}");

            references = await GetMetadataReferences();
            AppStateService.SetAssemblyAndCodeData(codeChallenges, references);
            isPageReady = true;
            StateHasChanged();
            
        }
        private async Task<IEnumerable<PortableExecutableReference>> GetMetadataReferences()
        {
            var sw = Stopwatch.StartNew();
            var refs = AppDomain.CurrentDomain.GetAssemblies();

            var narrowedRefs = refs.Where(x =>
                !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location) &&
                (x.GetName().Name == "System" || x.GetName().Name == "System.Core" || x.GetName().Name == "System.Numerics" || x.GetName().Name == "mscorlib" || x.GetName().Name == "netstandard"));

            var assemblyNames = narrowedRefs.Select(x => x.GetName().Name).Distinct().ToList();
            var streams = await PublicClient.GetAssemblyStreams(assemblyNames);
            var assemblyRefs = streams.Select(x => MetadataReference.CreateFromStream(x.Value));
            sw.Stop();
            Console.WriteLine("MetadataReferences Done in " + sw.ElapsedMilliseconds + "ms");
            return assemblyRefs;

        }
        protected Task HandleNotReady(int tab)
        {
            tabIndex = 0;
            StateHasChanged();
            return Task.CompletedTask;
        }

    }
}
