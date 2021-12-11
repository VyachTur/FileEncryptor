

using FileEncryptor.WPF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace FileEncryptor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static IHost __host;

        public static IHost Host => __host ??= Program.
            CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection service)
        {
            throw new NotImplementedException();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;

            base.OnStartup(e);

            await host.StartAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            using (Host) await Host.StopAsync();
        }

    }
}
