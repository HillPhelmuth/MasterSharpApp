using System;
using System.Collections.Generic;
using System.Text;
using MasterSharpOpen.Shared.CodeServices;
using MasterSharpOpen.Shared.StaticAuth;
using MasterSharpOpen.Shared.StaticAuth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MasterSharpOpen.Shared.ExtensionMethods
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMasterSharpServices(this IServiceCollection service)
        {
            service.AddSingleton<CodeEditorService>();
            service.AddSingleton<AppStateService>();
            service.AddScoped<ICustomAuthenticationStateProvider, CustomAuthenticationStateProvider>();
            return service;
        }
    }
}
