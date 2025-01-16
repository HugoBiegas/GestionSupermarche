using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;

namespace GestionSupermarche.Pages;

public partial class ConsultationPage : ContentPage
{
    private readonly TempsTravailRepository _tempsTravailRepository;
    private readonly EmployeRepository _employeRepository;
    private readonly RayonRepository _rayonRepository;
    private List<Employe> _employes;
    private Employe _selectedEmploye;
    public bool HasEmployeSelected => _selectedEmploye != null;

    public ConsultationPage()
    {
        InitializeComponent();
        _tempsTravailRepository = new TempsTravailRepository(Database.GetConnection());
        _employeRepository = new EmployeRepository(Database.GetConnection());
        _rayonRepository = new RayonRepository(Database.GetConnection());
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerEmployes();
    }

    private async void ChargerEmployes()
    {
        _employes = await _employeRepository.ObtenirTousLesEmployes();
        PickerEmploye.ItemsSource = _employes.Select(e => e.Nom).ToList();
    }

    private async void OnEmployeSelectionne(object sender, EventArgs e)
    {
        if (PickerEmploye.SelectedIndex == -1)
            return;

        _selectedEmploye = _employes[PickerEmploye.SelectedIndex];
        await ChargerTempsEmploye();
        OnPropertyChanged(nameof(HasEmployeSelected));
    }

    private async Task ChargerTempsEmploye()
    {
        var temps = await _tempsTravailRepository.ObtenirTempsParEmploye(_selectedEmploye.IdEmploye);
        var rayons = await _rayonRepository.ObtenirTousLesRayons();

        var tempsDetailles = temps.Select(t => new
        {
            t.IdTempsTravail,
            NomRayon = rayons.FirstOrDefault(r => r.IdRayon == t.IdRayon)?.Nom ?? "Inconnu",
            t.Date,
            t.Temps
        }).OrderByDescending(t => t.Date).ToList();

        ListViewTemps.ItemsSource = tempsDetailles;

        // Mettre à jour le total des heures
        double totalHeures = temps.Sum(t => t.Temps);
        LabelTotalHeures.Text = $"Total: {totalHeures}h";
    }

    private async void OnSupprimerTempsClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var tempsInfo = button?.CommandParameter as dynamic;

        if (tempsInfo == null) return;

        bool confirmation = await DisplayAlert("Confirmation",
            "Voulez-vous vraiment supprimer cet enregistrement ?",
            "Oui", "Non");

        if (confirmation)
        {
            var temps = new TempsTravail { IdTempsTravail = tempsInfo.IdTempsTravail };
            await _tempsTravailRepository.SupprimerTempsTravail(temps);
            await ChargerTempsEmploye();
        }
    }
}