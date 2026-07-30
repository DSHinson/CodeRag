using CodeRag.Cli.Screens.Components.Buttons;
using CodeRag.Cli.Screens.Components.StatusBar;
using CodeRag.Cli.Screens.Components.WelcomeBanner;
using Microsoft.Extensions.DependencyInjection;

namespace CodeRag.Cli.Screens.Components
{
    public static class AddScreenComponentsToServiceCollection
    {
        public static void AddScreenComponents(this IServiceCollection services)
        {
            services.AddTransient<IButtonComponent, ButtonComponent>();
            services.AddScoped<IStatusBar>(provider =>
            {
                if (OperatingSystem.IsWindows())
                {
                    return new WindowsStatusBar();
                }
                else
                {
                    throw new PlatformNotSupportedException("This application only supports Windows.");
                }
            });
            services.AddScoped<IWelcomeBanner>(provider =>
            {
                if (OperatingSystem.IsWindows())
                {
                    return new WindowsWelcomeBanner();
                }
                else
                {
                    throw new PlatformNotSupportedException("This application only supports Windows.");
                }
            });
        }
    }
}
