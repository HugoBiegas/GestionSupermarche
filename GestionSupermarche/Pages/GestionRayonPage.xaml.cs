using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;
using GestionSupermarche.DTO;

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
            _secteurs = await _secteurRepository.ObtenirTousLesSecteurs();
            PickerSecteur.ItemsSource = _secteurs.Select(s => s.Nom).ToList();
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
        var rayonsAvecSecteur = new List<RayonDto>();

        foreach (var rayon in rayons)
        {
            var secteur = _secteurs.FirstOrDefault(s => s.IdSecteur == rayon.IdSecteur);
            rayonsAvecSecteur.Add(new RayonDto
            {
                IdRayon = rayon.IdRayon,
                Nom = rayon.Nom,
                NomSecteur = secteur?.Nom ?? "Secteur inconnu",
                IdSecteur = rayon.IdSecteur

            });
        }

        ListViewRayons.ItemsSource = rayonsAvecSecteur.OrderBy(r => r.Nom);
    }

    private async void OnRayonSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is RayonDto rayonInfo)
        {
            var action = await DisplayActionSheet(
                "Que souhaitez-vous modifier ?",
                "Annuler",
                null,
                "Modifier le nom",
                "Modifier le secteur");

            switch (action)
            {
                case "Modifier le nom":
                    await ModifierNomRayon(rayonInfo);
                    break;
                case "Modifier le secteur":
                    await ModifierSecteurRayon(rayonInfo);
                    break;
            }
        }
        ((ListView)sender).SelectedItem = null;
    }

    private async Task ModifierNomRayon(RayonDto rayonInfo)
    {
        string nouveauNom = await DisplayPromptAsync(
            "Modifier le rayon",
            "Nom du rayon :",
            initialValue: rayonInfo.Nom,
            accept: "Modifier",
            cancel: "Annuler");

        if (!string.IsNullOrWhiteSpace(nouveauNom))
        {
            try
            {
                var rayonModifie = new Rayon
                {
                    IdRayon = rayonInfo.IdRayon,
                    Nom = nouveauNom.Trim(),
                    IdSecteur = rayonInfo.IdSecteur
                };

                await _rayonRepository.ModifierRayon(rayonModifie);
                await DisplayAlert("Succès", "Nom du rayon modifié avec succès", "OK");
                await ChargerRayons();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
            }
        }
    }

    private async Task ModifierSecteurRayon(RayonDto rayonInfo)
    {
        var secteurChoisi = await DisplayActionSheet(
            "Choisir le secteur",
            "Annuler",
            null,
            _secteurs.Select(s => s.Nom).ToArray());

        if (secteurChoisi != "Annuler" && secteurChoisi != null)
        {
            try
            {
                var secteurSelectionne = _secteurs.First(s => s.Nom == secteurChoisi);
                var rayonModifie = new Rayon
                {
                    IdRayon = rayonInfo.IdRayon,
                    Nom = rayonInfo.Nom,
                    IdSecteur = secteurSelectionne.IdSecteur
                };

                await _rayonRepository.ModifierRayon(rayonModifie);
                await DisplayAlert("Succès", "Secteur du rayon modifié avec succès", "OK");
                await ChargerRayons();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
            }
        }
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
            var button = sender as ImageButton;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerDonnees();
    }
}