using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;

namespace GestionSupermarche.Pages;

public partial class GestionRayonPage : ContentPage
{
    private readonly RayonRepository _rayonRepository;
    private readonly SecteurRepository _secteurRepository;
    private List<Secteur> _secteurs;

    public GestionRayonPage()
    {
        InitializeComponent();
        _rayonRepository = new RayonRepository(Database.GetConnection());
        _secteurRepository = new SecteurRepository(Database.GetConnection());
        ChargerDonnees();
    }

    private async void ChargerDonnees()
    {
        try
        {
            // Chargement des secteurs pour le Picker
            _secteurs = await _secteurRepository.ObtenirTousLesSecteurs();
            PickerSecteur.ItemsSource = _secteurs.Select(s => s.Nom).ToList();

            // Chargement des rayons avec leurs secteurs
            await ChargerRayons();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors du chargement des données: " + ex.Message, "OK");
        }
    }

    private async Task ChargerRayons()
    {
        var rayons = await _rayonRepository.ObtenirTousLesRayons();
        var rayonsAvecSecteur = new List<dynamic>();

        foreach (var rayon in rayons)
        {
            var secteur = _secteurs.FirstOrDefault(s => s.IdSecteur == rayon.IdSecteur);
            rayonsAvecSecteur.Add(new
            {
                rayon.IdRayon,
                rayon.Nom,
                NomSecteur = secteur?.Nom ?? "Secteur inconnu",
                rayon.IdSecteur
            });
        }

        ListViewRayons.ItemsSource = rayonsAvecSecteur.OrderBy(r => r.Nom);
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNomRayon.Text))
        {
            await DisplayAlert("Erreur", "Veuillez saisir un nom de rayon", "OK");
            return;
        }

        if (PickerSecteur.SelectedIndex == -1)
        {
            await DisplayAlert("Erreur", "Veuillez sélectionner un secteur", "OK");
            return;
        }

        try
        {
            var nouveauRayon = new Rayon
            {
                Nom = EntryNomRayon.Text.Trim(),
                IdSecteur = _secteurs[PickerSecteur.SelectedIndex].IdSecteur
            };

            await _rayonRepository.AjouterRayon(nouveauRayon);
            await ChargerRayons();

            // Réinitialisation des champs
            EntryNomRayon.Text = string.Empty;
            PickerSecteur.SelectedIndex = -1;

            await DisplayAlert("Succès", "Rayon ajouté avec succès", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors de l'ajout du rayon: " + ex.Message, "OK");
        }
    }

    private async void OnSupprimerClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var rayonInfo = button?.CommandParameter as dynamic;

            if (rayonInfo == null) return;

            bool confirmation = await DisplayAlert(
                "Confirmation",
                $"Voulez-vous vraiment supprimer le rayon {rayonInfo.Nom} ?",
                "Oui", "Non");

            if (confirmation)
            {
                var rayon = new Rayon { IdRayon = rayonInfo.IdRayon };
                await _rayonRepository.SupprimerRayon(rayon);
                await ChargerRayons();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors de la suppression: " + ex.Message, "OK");
        }
    }

    private void OnRayonSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Désélectionne l'item
        if (sender is ListView listView)
        {
            listView.SelectedItem = null;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerDonnees();
    }
}