using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;
using SQLitePCL;
using GestionSupermarche.DTO;

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
        var tempsList = temps.OrderByDescending(t => t.Date)
            .Take(10)
            .Select(t => new TempsTravailDetailDto
            {
                IdTempsTravail = t.IdTempsTravail,
                NomEmploye = _employes.FirstOrDefault(e => e.IdEmploye == t.IdEmploye)?.Nom ?? "Inconnu",
                NomRayon = _rayons.FirstOrDefault(r => r.IdRayon == t.IdRayon)?.Nom ?? "Inconnu",
                Date = t.Date,
                Temps = t.Temps
            })
            .ToList();

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
    private async void OnTempsSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is TempsTravailDetailDto tempsInfo)
        {
            var action = await DisplayActionSheet(
                "Que souhaitez-vous modifier ?",
                "Annuler",
                null,
                "Modifier l'employé",
                "Modifier le rayon",
                "Modifier la date",
                "Modifier les heures");

            switch (action)
            {
                case "Modifier l'employé":
                    await ModifierEmploye(tempsInfo);
                    break;
                case "Modifier le rayon":
                    await ModifierRayon(tempsInfo);
                    break;
                case "Modifier la date":
                    await ModifierDate(tempsInfo);
                    break;
                case "Modifier les heures":
                    await ModifierHeures(tempsInfo);
                    break;
            }
        }
    ((ListView)sender).SelectedItem = null;
    }

    private async Task ModifierEmploye(TempsTravailDetailDto tempsActuel)
    {
        var actionEmploye = await DisplayActionSheet(
            "Choisir le nouvel employé",
            "Annuler",
            null,
            _employes.Select(e => e.Nom).ToArray());

        if (actionEmploye != "Annuler" && actionEmploye != null)
        {
            try
            {
                var employe = _employes.First(e => e.Nom == actionEmploye);
                var temps = await _tempsTravailRepository.GetTempsTravailById(tempsActuel.IdTempsTravail);

                if (temps != null)
                {
                    temps.IdEmploye = employe.IdEmploye;
                    await _tempsTravailRepository.ModifierTempsTravail(temps);
                    await DisplayAlert("Succès", "Employé modifié avec succès", "OK");
                    await ChargerDerniersTemps();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
            }
        }
    }

    private async Task ModifierRayon(TempsTravailDetailDto tempsActuel)
    {
        var actionRayon = await DisplayActionSheet(
            "Choisir le nouveau rayon",
            "Annuler",
            null,
            _rayons.Select(r => r.Nom).ToArray());

        if (actionRayon != "Annuler" && actionRayon != null)
        {
            try
            {
                var rayon = _rayons.First(r => r.Nom == actionRayon);
                var temps = await _tempsTravailRepository.GetTempsTravailById(tempsActuel.IdTempsTravail);

                if (temps != null)
                {
                    temps.IdRayon = rayon.IdRayon;
                    await _tempsTravailRepository.ModifierTempsTravail(temps);
                    await DisplayAlert("Succès", "Rayon modifié avec succès", "OK");
                    await ChargerDerniersTemps();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
            }
        }
    }

    private async Task ModifierDate(TempsTravailDetailDto tempsActuel)
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
            Date = DateTime.Parse(tempsActuel.Date.ToString())
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
                temps.Date = datePicker.Date;
                await _tempsTravailRepository.ModifierTempsTravail(temps);
                await DisplayAlert("Succès", "Date modifiée avec succès", "OK");
                await Navigation.PopModalAsync();
                await ChargerDerniersTemps();
            }
        };

        btnAnnuler.Clicked += async (s, e) =>
        {
            await Navigation.PopModalAsync();
        };

        await Navigation.PushModalAsync(new NavigationPage(datePage));
    }

    private async Task ModifierHeures(TempsTravailDetailDto tempsActuel)
    {
        await AfficherPopupHeures(tempsActuel);
    }

    private async Task AfficherPopupHeures(TempsTravailDetailDto tempsActuel, string messageErreur = null)
    {
        var nouvellesHeures = await DisplayPromptAsync(
            "Modifier les heures",
            messageErreur ?? "Entrez le nombre d'heures (0-24):",
            initialValue: ((int)tempsActuel.Temps).ToString(),
            accept: "Modifier",
            cancel: "Annuler",
            keyboard: Keyboard.Numeric,
            maxLength: 2);

        if (string.IsNullOrWhiteSpace(nouvellesHeures))
            return;

        if (!int.TryParse(nouvellesHeures, out int heures) || heures < 0 || heures > 24)
        {
            // Réafficher la popup avec un message d'erreur
            await AfficherPopupHeures(tempsActuel, "Erreur: Veuillez entrer un nombre entre 0 et 24");
            return;
        }

        try
        {
            var temps = await _tempsTravailRepository.GetTempsTravailById(tempsActuel.IdTempsTravail);
            if (temps != null)
            {
                temps.Temps = heures;
                await _tempsTravailRepository.ModifierTempsTravail(temps);
                await DisplayAlert("Succès", "Heures modifiées avec succès", "OK");
                await ChargerDerniersTemps();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
            await AfficherPopupHeures(tempsActuel);
        }
    }
}