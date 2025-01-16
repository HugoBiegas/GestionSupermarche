using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace GestionSupermarche
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Initialisation de la base de données SQLite
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "supermarche.db");
            //builder.Services.AddSingleton<Services.Database>(s => ActivatorUtilities.CreateInstance<Services.Database>(s, dbPath));


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
