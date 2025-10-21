using Microsoft.EntityFrameworkCore;
using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    public static class ObjectRepository
    {
        public static IDataProvider DataProvider { get; private set; } = null!;
        public static AppConfiguration AppConfiguration { get; private set; } = null!;
        public static ISendNotification NotificationService { get; private set; } = null!;

        public static void Initialize(
            IDataProvider dataProvider,
            AppConfiguration appConfiguration,
            ISendNotification notificationService)
        {
            DataProvider = dataProvider;
            AppConfiguration = appConfiguration;
            NotificationService = notificationService;
        }
        static ObjectRepository()
        {
            var options = new DbContextOptionsBuilder<ProjectManagerContext>()
                .UseSqlServer(App.DBConnectionString)
                .Options;

            DataProvider = new EfDataProvider(new ProjectManagerContext(options));

            //DataProvider = new SqlDataProvider();

            AppConfiguration = new AppConfiguration();
            NotificationService = new EmailService();
        }
    }
}
