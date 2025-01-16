using GestionSupermarche.Services;

namespace GestionSupermarche
{
    public partial class App : Application
    {
        private static Database database;
        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database();
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                database = new Database();
                await database.InitializeAsync();
            }).Wait();

            MainPage = new AppShell();
        }


    }
}
