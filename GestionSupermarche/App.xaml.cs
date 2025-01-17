using GestionSupermarche.Services;

namespace GestionSupermarche
{
    public partial class App : Application
    {
        private readonly ServiceAudio _serviceAudio;
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

        public App(ServiceAudio serviceAudio)
        {
            InitializeComponent();

            Task.Run(async () =>
            {
                database = new Database();
                await database.InitializeAsync();
            }).Wait();

            _serviceAudio = serviceAudio;
            
            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);

            window.Activated += Window_Activated;
            window.Deactivated += Window_Deactivated;
            window.Stopped += Window_Stopped;
            window.Resumed += Window_Resumed;
            window.Destroying += Window_Destroying;

            return window;
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            await _serviceAudio.Reprendre();
        }

        private async void Window_Deactivated(object sender, EventArgs e)
        {
            await _serviceAudio.MettreEnPause();
        }

        private async void Window_Stopped(object sender, EventArgs e)
        {
            await _serviceAudio.MettreEnPause();
        }

        private async void Window_Resumed(object sender, EventArgs e)
        {
            await _serviceAudio.Reprendre();
        }

        private void Window_Destroying(object sender, EventArgs e)
        {
            _serviceAudio.Arreter();
        }

    }
}
