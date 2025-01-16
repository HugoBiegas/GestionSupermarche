using GestionSupermarche.Repositories;
using GestionSupermarche.Services;


namespace GestionSupermarche.Pages;

public partial class StatistiquesPage : ContentPage
{
    private readonly TempsTravailRepository _tempsTravailRepository;
    private readonly EmployeRepository _employeRepository;
    private readonly RayonRepository _rayonRepository;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ChargerStatistiques();
    }

    public StatistiquesPage()
    {
        InitializeComponent();
        _tempsTravailRepository = new TempsTravailRepository(Database.GetConnection());
        _employeRepository = new EmployeRepository(Database.GetConnection());
        _rayonRepository = new RayonRepository(Database.GetConnection());
        ChargerStatistiques();
    }

    private async void ChargerStatistiques()
    {
        await ChargerHeuresParEmploye();
        await ChargerHeuresParRayon();
        await ChargerHeuresParMois();
        await ChargerTotalGeneral();
    }

    private async Task ChargerHeuresParEmploye()
    {
        var employes = await _employeRepository.ObtenirTousLesEmployes();
        var statistiques = new List<dynamic>();

        foreach (var employe in employes)
        {
            double totalHeures = await _tempsTravailRepository.CalculerTotalHeuresEmploye(employe.IdEmploye);
            statistiques.Add(new
            {
                employe.Nom,
                TotalHeures = totalHeures
            });
        }

        ListViewHeuresEmployes.ItemsSource = statistiques.OrderByDescending(s => s.TotalHeures);
    }

    private async Task ChargerHeuresParRayon()
    {
        var rayons = await _rayonRepository.ObtenirTousLesRayons();
        var statistiques = new List<dynamic>();

        foreach (var rayon in rayons)
        {
            double totalHeures = await _rayonRepository.CalculerTotalHeuresParRayon(rayon.IdRayon);
            statistiques.Add(new
            {
                rayon.Nom,
                TotalHeures = totalHeures
            });
        }

        ListViewHeuresRayons.ItemsSource = statistiques.OrderByDescending(s => s.TotalHeures);
    }

    private async Task ChargerHeuresParMois()
    {
        var temps = await _tempsTravailRepository.ObtenirTousLesTemps();
        var statistiques = temps
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new
            {
                Mois = $"{g.Key.Month:00}/{g.Key.Year}",
                TotalHeures = g.Sum(t => t.Temps)
            })
            .OrderByDescending(s => s.Mois)
            .ToList();

        ListViewHeuresMois.ItemsSource = statistiques;
    }

    private async Task ChargerTotalGeneral()
    {
        double totalGeneral = await _tempsTravailRepository.CalculerTotalHeuresGeneral();
        LabelTotalGeneral.Text = $"{totalGeneral}h";
    }

    private async void OnRafraichirClicked(object sender, EventArgs e)
    {
        try
        {
            IsBusy = true;

            ChargerStatistiques();

            await DisplayAlert("Succès", "Les statistiques ont été mises à jour", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Une erreur est survenue lors du rafraîchissement des statistiques", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}