using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectManager.DataProvider;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ProjectManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DBConnectionString
        {
            get => @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\josua\ProjectManger.mdf;Integrated Security=True;Connect Timeout=60;Max Pool Size=200;";
        }
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Connection-String aus App.DBConnectionString oder appsettings.json
                    services.AddDbContext<ProjectManagerContext>(options =>
                        options.UseSqlServer(App.DBConnectionString));

                    services.AddSingleton<IDataProvider, EfDataProvider>();
                    services.AddSingleton<AppConfiguration>();
                    services.AddSingleton<ISendNotification, EmailService>();

                    // Optional: ViewModels, Windows, Services
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }


    }

}
