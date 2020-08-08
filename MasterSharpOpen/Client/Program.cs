using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Blazor.ModalDialog;
using MasterSharpOpen.Shared;
using MasterSharpOpen.Shared.CodeServices;
using MasterSharpOpen.Shared.ExtensionMethods;
using MasterSharpOpen.Shared.StaticAuth;
using MasterSharpOpen.Shared.StaticAuth.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Console;
using TextCopy;

namespace MasterSharpOpen.Client
{
    [Log(AttributeExclude = true)]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var backend = new ConsoleLoggingBackend();
            backend.Options.Theme = ConsoleThemes.Dark;
            LoggingServices.DefaultBackend = backend;
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }); 
            builder.Services.AddHttpClient<PublicClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddAuthentication();
            builder.Services.AddMasterSharpServices();
           // builder.Services.AddSingleton<CodeEditorService>();
            //builder.Services.AddSingleton<AppStateService>();
            builder.Services.InjectClipboard();
            builder.Services.AddModalDialog();
            //builder.Services.AddScoped<ICustomAuthenticationStateProvider, CustomAuthenticationStateProvider>();
            await builder.Build().RunAsync();
        }
    }
}
