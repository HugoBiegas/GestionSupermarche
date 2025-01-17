using GestionSupermarche.Models;
using GestionSupermarche.Repositories;
using GestionSupermarche.Services;

namespace GestionSupermarche.Pages;

public partial class GestionEmployePage : ContentPage
{
    private readonly EmployeRepository _employeRepository;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ChargerEmployes();
    }

    public GestionEmployePage()
    {
        InitializeComponent();
        _employeRepository = new EmployeRepository(Database.GetConnection());

        ChargerEmployes();
    }

    private async void ChargerEmployes()
    {
        ListViewEmployes.ItemsSource = await _employeRepository.ObtenirTousLesEmployes();
    }

    private async void OnEmployeSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Employe employe)
        {
            string nouveauNom = await DisplayPromptAsync(
                "Modifier l'employé",
                "Nom de l'employé :",
                initialValue: employe.Nom,
                accept: "Modifier",
                cancel: "Annuler");

            if (!string.IsNullOrWhiteSpace(nouveauNom))
            {
                try
                {
                    employe.Nom = nouveauNom.Trim();
                    await _employeRepository.ModifierEmploye(employe);
                    await DisplayAlert("Succès", "Employé modifié avec succès", "OK");
                    ChargerEmployes();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", $"Erreur lors de la modification : {ex.Message}", "OK");
                }
            }
        }
        ((ListView)sender).SelectedItem = null;
    }

    private async void OnAjouterEmployeClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryNomEmploye.Text))
        {
            await DisplayAlert("Erreur", "Veuillez saisir un nom", "OK");
            return;
        }

        var nouvelEmploye = new Employe { Nom = EntryNomEmploye.Text };
        await _employeRepository.AjouterEmploye(nouvelEmploye);

        EntryNomEmploye.Text = string.Empty;
        ChargerEmployes();
    }

    private async void OnSupprimerEmployeClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var employe = button?.CommandParameter as Employe;

        if (employe == null) return;

        bool reponse = await DisplayAlert("Confirmation",
            $"Voulez-vous vraiment supprimer {employe.Nom} ?",
            "Oui", "Non");

        if (reponse)
        {
            await _employeRepository.SupprimerEmploye(employe);
            ChargerEmployes();
        }
    }
}