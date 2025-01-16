
namespace GestionSupermarche
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Enregistrement des routes pour la navigation
            Routing.RegisterRoute(nameof(Pages.MainPage), typeof(Pages.MainPage));
            Routing.RegisterRoute(nameof(Pages.GestionEmployePage), typeof(Pages.GestionEmployePage));
            Routing.RegisterRoute(nameof(Pages.GestionRayonPage), typeof(Pages.GestionRayonPage));
            Routing.RegisterRoute(nameof(Pages.GestionSecteurPage), typeof(Pages.GestionSecteurPage));
            Routing.RegisterRoute(nameof(Pages.SaisieTempsPage), typeof(Pages.SaisieTempsPage));
            Routing.RegisterRoute(nameof(Pages.ConsultationPage), typeof(Pages.ConsultationPage));
            Routing.RegisterRoute(nameof(Pages.StatistiquesPage), typeof(Pages.StatistiquesPage));
            Routing.RegisterRoute(nameof(Pages.GraphiquePage), typeof(Pages.GraphiquePage));
        }
    }
}
