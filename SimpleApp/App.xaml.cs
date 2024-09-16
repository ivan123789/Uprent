using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace SimpleApp
{
    public partial class App : Application
    {
        public IConfiguration Configuration { get; }
        private IServiceProvider _serviceProvider;

        public App()
        {
            // Configuration for appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Read base URL from configuration
            var baseUrl = Configuration["ApiBaseUrl"]; // Ensure ApiBaseUrl is defined in appSettings.json
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("API base URL is not configured.");
            }

            services.AddSingleton(new HttpService(baseUrl));
            services.AddScoped<MainWindow>();
            services.AddScoped<UserRolesWindow>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}