using GestionSupermarche.DTO;
using GestionSupermarche.Models;
using GestionSupermarche.Repositories;
using GestionSupermarche.Services;

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
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ReinitialiserChamps();
        ChargerEmployes();
    }

    private void ReinitialiserChamps()
    {
        PickerEmploye.SelectedIndex = -1;
        ListViewTemps.ItemsSource = null;
        LabelTotalHeures.Text = "Total: 0h";
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

        var tempsDetailles = temps.Select(t => new TempsTravailDto
        {
            IdTempsTravail = t.IdTempsTravail,
            NomRayon = rayons.FirstOrDefault(r => r.IdRayon == t.IdRayon)?.Nom ?? "Inconnu",
            Date = t.Date,
            Temps = t.Temps
        }).OrderByDescending(t => t.Date).ToList();

        ListViewTemps.ItemsSource = tempsDetailles;

        // Mettre à jour le total des heures
        double totalHeures = temps.Sum(t => t.Temps);
        LabelTotalHeures.Text = $"Total: {totalHeures}h";
    }

    private async void OnSupprimerTempsClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var tempsInfo = button?.CommandParameter as TempsTravailDto;

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

    private async void OnTempsSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is TempsTravailDto tempsInfo)
        {
            var action = await DisplayActionSheet(
                "Que souhaitez-vous modifier ?",
                "Annuler",
                null,
                "Modifier la date",
                "Modifier le nombre d'heures");

            switch (action)
            {
                case "Modifier la date":
                    await ModifierDate(tempsInfo);
                    break;
                case "Modifier le nombre d'heures":
                    await ModifierHeures(tempsInfo);
                    break;
            }
        }
        ((ListView)sender).SelectedItem = null;
    }

    private async Task ModifierDate(TempsTravailDto tempsActuel)
    {
        try
        {
            var datePage = new ContentPage();
            var dateStack = new VerticalStackLayout
            {
                Padding = new Thickness(20),
                Spacing = 10
            };

            var datePicker = new DatePicker
            {
                Format = "dd/MM/yyyy",
                Date = tempsActuel.Date
            };

            var btnValider = new Button
            {
                Text = "Valider",
                BackgroundColor = Colors.Blue,
                TextColor = Colors.White
            };

            var btnAnnuler = new Button
            {
                Text = "Annuler",
                BackgroundColor = Colors.Gray,
                TextColor = Colors.White
            };

            dateStack.Children.Add(datePicker);
            dateStack.Children.Add(btnValider);
            dateStack.Children.Add(btnAnnuler);
            datePage.Content = dateStack;

            btnValider.Clicked += async (s, e) =>
            {
                var temps = await _tempsTravailRepository.GetTempsTravailById(tempsActuel.IdTempsTravail);
                if (temps != null)
                {
                    bool disponible = await _tempsTravailRepository.VerifierDisponibiliteEmploye(
                        _selectedEmploye.IdEmploye, temps.IdRayon, datePicker.Date);

                    if (!disponible)
                    {
                        await DisplayAlert("Erreur", "L'employé travaille déjà dans ce rayon à cette date", "OK");
                        return;
                    }

                    temps.Date = datePicker.Date;
                    await _tempsTravailRepository.ModifierTempsTravail(temps);
                    await DisplayAlert("Succès", "Date modifiée avec succès", "OK");
                    await Navigation.PopModalAsync();
                    await ChargerTempsEmploye();
                }
            };

            btnAnnuler.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();
            };

            await Navigation.PushModalAsync(new NavigationPage(datePage));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
        }
    }

    private async Task ModifierHeures(TempsTravailDto tempsActuel)
    {
        try
        {
            var nouvellesHeures = await DisplayPromptAsync(
                "Modifier les heures",
                "Entrez le nombre d'heures (0-24):",
                initialValue: ((int)tempsActuel.Temps).ToString(),
                accept: "Modifier",
                cancel: "Annuler",
                keyboard: Keyboard.Numeric,
                maxLength: 2);

            if (string.IsNullOrWhiteSpace(nouvellesHeures))
                return;

            if (!int.TryParse(nouvellesHeures, out int heures) || heures < 0 || heures > 24)
            {
                await DisplayAlert("Erreur", "Veuillez entrer un nombre entre 0 et 24", "OK");
                await ModifierHeures(tempsActuel);
                return;
            }

            var temps = await _tempsTravailRepository.GetTempsTravailById(tempsActuel.IdTempsTravail);
            if (temps != null)
            {
                temps.Temps = heures;
                await _tempsTravailRepository.ModifierTempsTravail(temps);
                await DisplayAlert("Succès", "Heures modifiées avec succès", "OK");
                await ChargerTempsEmploye();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
        }
    }

}