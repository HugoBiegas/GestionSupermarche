using Microcharts;
using SkiaSharp;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;

namespace GestionSupermarche.Pages;

public partial class GraphiquePage : ContentPage
{
    private readonly TempsTravailRepository _tempsTravailRepository;
    private readonly EmployeRepository _employeRepository;
    private readonly RayonRepository _rayonRepository;
    private readonly string[] _couleurs = new[]
    {
        "#2c3e50",  // Bleu foncé
        "#e74c3c",  // Rouge
        "#2ecc71",  // Vert
        "#f1c40f",  // Jaune
        "#9b59b6",  // Violet
        "#3498db",  // Bleu clair
        "#e67e22"   // Orange
    };
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerGraphiques();
    }

    public GraphiquePage()
    {
        InitializeComponent();

        // Initialisation des repositories
        _tempsTravailRepository = new TempsTravailRepository(Database.GetConnection());
        _employeRepository = new EmployeRepository(Database.GetConnection());
        _rayonRepository = new RayonRepository(Database.GetConnection());

        ChargerGraphiques();
    }


    private async void ChargerGraphiques()
    {
        try
        {
            await ChargerGraphiqueEmployes();
            await ChargerGraphiqueRayons();
            await ChargerGraphiqueMensuel();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Une erreur est survenue lors du chargement des graphiques.", "OK");
        }
    }

    private async Task ChargerGraphiqueEmployes()
    {
        var employes = await _employeRepository.ObtenirTousLesEmployes();
        List<ChartEntry> entries = new List<ChartEntry>();

        for (int i = 0; i < employes.Count; i++)
        {
            float heures = (float)await _tempsTravailRepository
                .CalculerTotalHeuresEmploye(employes[i].IdEmploye);

            entries.Add(new ChartEntry(heures)
            {
                Label = employes[i].Nom,
                ValueLabel = heures.ToString("F1"),
                Color = SKColor.Parse(_couleurs[i % _couleurs.Length]),
                TextColor = SKColors.Gray,
                ValueLabelColor = SKColors.Black
            });
        }

        ChartViewEmployes.Chart = new BarChart { Entries = entries.ToArray() };
    }


    private async Task ChargerGraphiqueRayons()
    {
        var rayons = await _rayonRepository.ObtenirTousLesRayons();
        List<ChartEntry> entries = new List<ChartEntry>();

        for (int i = 0; i < rayons.Count; i++)
        {
            float heures = (float)await _rayonRepository
                .CalculerTotalHeuresParRayon(rayons[i].IdRayon);

            entries.Add(new ChartEntry(heures)
            {
                Label = rayons[i].Nom,
                ValueLabel = heures.ToString("F1"),
                Color = SKColor.Parse(_couleurs[i % _couleurs.Length]),
                TextColor = SKColors.Gray,
                ValueLabelColor = SKColors.Black
            });
        }

        ChartViewRayons.Chart = new DonutChart { Entries = entries.ToArray() };
    }

    private async Task ChargerGraphiqueMensuel()
    {
        var temps = await _tempsTravailRepository.ObtenirTousLesTemps();
        var donneesMensuelles = temps
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Take(12)
            .ToList();

        List<ChartEntry> entries = new List<ChartEntry>();

        for (int i = 0; i < donneesMensuelles.Count; i++)
        {
            var mois = donneesMensuelles[i];
            float totalHeures = (float)mois.Sum(t => t.Temps);

            entries.Add(new ChartEntry(totalHeures)
            {
                Label = $"{mois.Key.Month:00}/{mois.Key.Year}",
                ValueLabel = totalHeures.ToString("F1"),
                Color = SKColor.Parse(_couleurs[0]),
                TextColor = SKColors.Gray,
                ValueLabelColor = SKColors.Black
            });
        }

        ChartViewMensuel.Chart = new LineChart { Entries = entries.ToArray() };
    }

    private async void OnRefreshButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Afficher un indicateur de chargement
            IsBusy = true;

            await Task.Run(ChargerGraphiques);

            await DisplayAlert("Succès",
                "Les graphiques ont été actualisés avec succès.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Une erreur est survenue lors de l'actualisation.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}