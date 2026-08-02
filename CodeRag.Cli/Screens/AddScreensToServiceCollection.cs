using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens
{
    public static class AddScreensToServiceCollection
    {
        public static void AddScreens(this IServiceCollection services)
        {
            services.AddSingleton<IScreen,WelcomeScreen>();
            services.AddSingleton<IScreen, IndexingScreen>();
        }
    }
}
