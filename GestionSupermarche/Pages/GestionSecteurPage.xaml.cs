using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;

namespace GestionSupermarche.Pages;

public partial class GestionSecteurPage : ContentPage
{
    private readonly SecteurRepository _secteurRepository;
    private readonly RayonRepository _rayonRepository;
    public GestionSecteurPage()
    {
        InitializeComponent();
        _secteurRepository = new SecteurRepository(Database.GetConnection());
        _rayonRepository = new RayonRepository(Database.GetConnection());
        ChargerSecteurs();
    }

    private async void ChargerSecteurs()
    {
        try
        {
            var secteurs = await _secteurRepository.ObtenirTousLesSecteurs();
            var secteursAvecRayons = new List<dynamic>();

            foreach (var secteur in secteurs)
            {
                var rayons = await _rayonRepository.ObtenirRayonsParSecteur(secteur.IdSecteur);
                secteursAvecRayons.Add(new
                {
                    secteur.IdSecteur,
                    secteur.Nom,
                    NombreRayons = rayons.Count,
                    PeutSupprimer = rayons.Count == 0
                });
            }

            ListViewSecteurs.ItemsSource = secteursAvecRayons.OrderBy(s => s.Nom);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors du chargement des secteurs: " + ex.Message, "OK");
        }
    }

    private async void OnAjouterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNomSecteur.Text))
        {
            await DisplayAlert("Erreur", "Veuillez saisir un nom de secteur", "OK");
            return;
        }

        try
        {
            var nouveauSecteur = new Secteur
            {
                Nom = EntryNomSecteur.Text.Trim()
            };

            await _secteurRepository.AjouterSecteur(nouveauSecteur);
            EntryNomSecteur.Text = string.Empty;
            ChargerSecteurs();

            await DisplayAlert("Succès", "Secteur ajouté avec succès", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors de l'ajout du secteur: " + ex.Message, "OK");
        }
    }

    private async void OnSupprimerClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var secteurInfo = button?.CommandParameter as dynamic;

            if (secteurInfo == null || !secteurInfo.PeutSupprimer) return;

            bool confirmation = await DisplayAlert(
                "Confirmation",
                $"Voulez-vous vraiment supprimer le secteur {secteurInfo.Nom} ?",
                "Oui", "Non");

            if (confirmation)
            {
                var secteur = new Secteur { IdSecteur = secteurInfo.IdSecteur };
                await _secteurRepository.SupprimerSecteur(secteur);
                ChargerSecteurs();
                await DisplayAlert("Succès", "Secteur supprimé avec succès", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", "Erreur lors de la suppression du secteur: " + ex.Message, "OK");
        }
    }

    private void OnSecteurSelected(object sender, SelectedItemChangedEventArgs e)
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
        ChargerSecteurs();
    }
}