using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectManager.DataProvider;
using ProjectManager.Model;
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
            get => @"Server=localhost\SQLEXPRESS;Database=ProjectManager;Trusted_Connection=True;TrustServerCertificate=True;";
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
            using var scope = App.AppHost.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectManagerContext>();

            // Erstellt die Datenbank, falls sie nicht existiert
            context.Database.EnsureCreated();

            var dataProvider = scope.ServiceProvider.GetRequiredService<IDataProvider>();
            var config = scope.ServiceProvider.GetRequiredService<AppConfiguration>();
            var notifier = scope.ServiceProvider.GetRequiredService<ISendNotification>();

            ObjectRepository.Initialize(dataProvider, config, notifier);

            // Default-Benutzer anlegen
            if (!ObjectRepository.DataProvider.GetUsers().Result.Any())
            {
                await ObjectRepository.DataProvider.CreateUser(new User("admin", "admin123")
                {
                    Firstname = "System",
                    Lastname = "Administrator",
                    Email = "admin@localhost",
                    IsAdmin = true
                });
            }


            // LoginWindow anzeigen
            var loginWindow = new LoginWindow(); // Optional: über DI holen, falls du Services brauchst
            var loginResult = loginWindow.ShowDialog(); // Modal anzeigen

            // Prüfen, ob Login erfolgreich war
            if (ObjectRepository.DataProvider.CurrentUser != null)
            {
                var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Kein Benutzer angemeldet. Die Anwendung wird beendet.", "Anmeldung fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }


    }

}
