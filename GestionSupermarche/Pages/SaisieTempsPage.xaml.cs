using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;
using SQLitePCL;

namespace GestionSupermarche.Pages;

public partial class SaisieTempsPage : ContentPage
{
    private readonly TempsTravailRepository _tempsTravailRepository;
    private readonly EmployeRepository _employeRepository;
    private readonly RayonRepository _rayonRepository;
    private List<Employe> _employes;
    private List<Rayon> _rayons;
    private string _heuresText;
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerDonnees();
    }

    public SaisieTempsPage()
    {
        InitializeComponent();
        var connection = Database.GetConnection();
        _tempsTravailRepository = new TempsTravailRepository(connection);
        _employeRepository = new EmployeRepository(connection);
        _rayonRepository = new RayonRepository(connection);

        DatePickerTravail.Date = DateTime.Today;
    }

    private async void ChargerDonnees()
    {
        try
        {
            _employes = await _employeRepository.ObtenirTousLesEmployes();
            PickerEmploye.ItemsSource = _employes.Select(e => e.Nom).ToList();

            _rayons = await _rayonRepository.ObtenirTousLesRayons();
            PickerRayon.ItemsSource = _rayons.Select(r => r.Nom).ToList();

            await ChargerDerniersTemps();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }


    private async Task ChargerDerniersTemps()
    {
        var temps = await _tempsTravailRepository.ObtenirTousLesTemps();
        var tempsList = new List<dynamic>();

        foreach (var t in temps.OrderByDescending(t => t.Date).Take(10))
        {
            var employe = _employes.FirstOrDefault(e => e.IdEmploye == t.IdEmploye);
            var rayon = _rayons.FirstOrDefault(r => r.IdRayon == t.IdRayon);
            tempsList.Add(new
            {
                NomEmploye = employe?.Nom ?? "Inconnu",
                NomRayon = rayon?.Nom ?? "Inconnu",
                t.Date,
                t.Temps
            });
        }

        ListViewDerniersTemps.ItemsSource = tempsList;
    }

    private async void OnEnregistrerClicked(object sender, EventArgs e)
    {
        if (PickerEmploye.SelectedIndex == -1 || PickerRayon.SelectedIndex == -1)
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner un employé et un rayon", "OK");
            return;
        }

        if (!double.TryParse(EntryHeures.Text, out double heures) || heures <= 0 || heures > 24)
        {
            await DisplayAlert("Erreur", "Veuillez saisir un nombre d'heures valide (entre 1 et 24)", "OK");
            return;
        }

        var idEmploye = _employes[PickerEmploye.SelectedIndex].IdEmploye;
        var idRayon = _rayons[PickerRayon.SelectedIndex].IdRayon;

        try
        {
            var nouveauTemps = new TempsTravail
            {
                IdEmploye = idEmploye,
                IdRayon = idRayon,
                Date = DatePickerTravail.Date,
                Temps = heures
            };

            await _tempsTravailRepository.AjouterTempsTravail(nouveauTemps);

            ReinitalisationDesChamps();

            await ChargerDerniersTemps();

            await DisplayAlert("Succès", "Temps de travail enregistré", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private void ReinitalisationDesChamps()
    {
        PickerEmploye.SelectedIndex = -1;
        PickerRayon.SelectedIndex = -1;
        EntryHeures.Text = string.Empty;
        DatePickerTravail.Date = DateTime.Today;
    }
    public string HeuresText
    {
        get => _heuresText;
        set
        {
            if (_heuresText != value)
            {
                _heuresText = value;
                OnPropertyChanged(nameof(HeuresText));
            }
        }
    }
}