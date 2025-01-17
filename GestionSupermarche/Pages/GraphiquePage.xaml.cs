using GestionSupermarche.Repositories;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using GestionSupermarche.Services;

namespace GestionSupermarche.Pages;

public partial class GraphiquePage : ContentPage
{
    private readonly TempsTravailRepository _tempsTravailRepository;
    private readonly EmployeRepository _employeRepository;
    private readonly RayonRepository _rayonRepository;

    public GraphiquePage()
    {
        InitializeComponent();
        _tempsTravailRepository = new TempsTravailRepository(Database.GetConnection());
        _employeRepository = new EmployeRepository(Database.GetConnection());
        _rayonRepository = new RayonRepository(Database.GetConnection());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerGraphiques();
    }

    private async void ChargerGraphiques()
    {
        await ChargerGraphiqueHeuresMois();
        await ChargerGraphiqueHeuresEmployes();
        await ChargerGraphiqueHeuresRayons();
    }

    private async Task ChargerGraphiqueHeuresMois()
    {
        try
        {
            var temps = await _tempsTravailRepository.ObtenirTousLesTemps();
            var donnees = temps
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Mois = $"{g.Key.Month:00}/{g.Key.Year}",
                    TotalHeures = g.Sum(t => t.Temps)
                })
                .OrderBy(s => s.Mois)
                .ToList();

            var model = new PlotModel
            {
                Title = "Heures par Mois",
                TextColor = OxyColors.Black
            };

            var axeX = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Mois",
                TitleColor = OxyColors.Black,
                TicklineColor = OxyColors.Black,
                Angle = 45,
                IsZoomEnabled = false
            };

            foreach (var item in donnees)
            {
                axeX.Labels.Add(item.Mois);
            }

            var maxHeures = donnees.Max(d => d.TotalHeures);
            var maxArrondi = Math.Ceiling(maxHeures / 10.0) * 10 + 10;

            var axeY = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Heures",
                TitleColor = OxyColors.Black,
                TicklineColor = OxyColors.Black,
                Minimum = 0,
                Maximum = maxArrondi,
                MajorStep = 50, 
                MinorStep = 10,  
                IsZoomEnabled = false,
                StringFormat = "0",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                AxisDistance = 20,
                AxislineStyle = LineStyle.Solid,
                AxislineThickness = 1
            };

            model.Axes.Add(axeX);
            model.Axes.Add(axeY);

            var lineSeries = new LineSeries
            {
                Title = "Heures",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerStroke = OxyColors.Blue,
                MarkerFill = OxyColors.White,
                StrokeThickness = 2,
                LabelFormatString = "{1:0}h",
                TrackerFormatString = "Mois: {2}\nHeures: {4:0}",
                LineStyle = LineStyle.Solid
            };

            for (int i = 0; i < donnees.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, donnees[i].TotalHeures));
            }

            model.Series.Add(lineSeries);

            model.IsLegendVisible = false;
            model.PlotMargins = new OxyThickness(40, 20, 20, 40);
            model.PlotAreaBorderThickness = new OxyThickness(1);
            model.PlotAreaBorderColor = OxyColors.Black;

            GraphiqueHeuresMois.Model = model;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Erreur lors du chargement du graphique des heures par mois: " + ex.Message,
                "OK");
        }
    }

    private async Task ChargerGraphiqueHeuresEmployes()
    {
        try
        {
            var employes = await _employeRepository.ObtenirTousLesEmployes();
            var model = new PlotModel
            {
                Title = "Répartition des heures par Employé",
                TextColor = OxyColors.Black
            };

            var axeY = new CategoryAxis
            {
                Position = AxisPosition.Left,
                Title = "Employés",
                TitleColor = OxyColors.Black,
                TicklineColor = OxyColors.Black,
                IsZoomEnabled = false,
            };

            var donnees = new List<double>();
            foreach (var employe in employes)
            {
                double totalHeures = await _tempsTravailRepository
                    .CalculerTotalHeuresEmploye(employe.IdEmploye);
                donnees.Add(totalHeures);
                axeY.Labels.Add(employe.Nom);
            }

            var maxHeures = donnees.Max();
            var maxArrondi = Math.Ceiling(maxHeures / 50.0) * 50 + 50;

            var axeX = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Heures",
                TitleColor = OxyColors.Black,
                TicklineColor = OxyColors.Black,
                Minimum = 0,
                Maximum = maxArrondi,
                MajorStep = 50,
                MinorStep = 10,
                IsZoomEnabled = false,
                StringFormat = "0",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                AxisDistance = 20,
                AxislineStyle = LineStyle.Solid,
                AxislineThickness = 1,
                MajorGridlineThickness = 1,
                MinorGridlineThickness = 0.5,
                TextColor = OxyColors.Black
            };

            model.Axes.Add(axeY);
            model.Axes.Add(axeX);

            var series = new BarSeries
            {
                FillColor = OxyColors.LightBlue,
                StrokeColor = OxyColors.Blue,
                StrokeThickness = 1,
                BarWidth = 0.8,
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}h"
            };

            for (int i = 0; i < donnees.Count; i++)
            {
                series.Items.Add(new BarItem { Value = donnees[i] });
            }

            model.Series.Add(series);

            model.IsLegendVisible = false;
            model.PlotMargins = new OxyThickness(60, 20, 20, 40);
            model.PlotAreaBorderThickness = new OxyThickness(1);
            model.PlotAreaBorderColor = OxyColors.Black;

            GraphiqueHeuresEmployes.Model = model;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Erreur lors du chargement du graphique des heures par employé: " + ex.Message,
                "OK");
        }
    }

    private async Task ChargerGraphiqueHeuresRayons()
    {
        try
        {
            var rayons = await _rayonRepository.ObtenirTousLesRayons();
            var model = new PlotModel
            {
                Title = "Répartition par Rayon",
                TextColor = OxyColors.Black
            };

            var series = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (var rayon in rayons)
            {
                double totalHeures = await _rayonRepository
                    .CalculerTotalHeuresParRayon(rayon.IdRayon);

                series.Slices.Add(new PieSlice(rayon.Nom, totalHeures)
                {
                    IsExploded = true
                });
            }

            model.Series.Add(series);
            GraphiqueHeuresRayons.Model = model;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Erreur lors du chargement du graphique des heures par rayon: " + ex.Message,
                "OK");
        }
    }

    private async void OnRafraichirClicked(object sender, EventArgs e)
    {
        try
        {
            IsBusy = true;
            ChargerGraphiques();
            await DisplayAlert("Succès", "Les graphiques ont été mis à jour", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur",
                "Une erreur est survenue lors du rafraîchissement des graphiques: " + ex.Message,
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}