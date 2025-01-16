namespace GestionSupermarche.Pages;
using GestionSupermarche.Models;
using GestionSupermarche.Services;
using GestionSupermarche.Repositories;
public partial class GestionEmployePage : ContentPage
{
    private readonly EmployeRepository _employeRepository;

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
        var button = sender as Button;
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

    private void OnEmployeSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // Désélectionne l'item
        ((ListView)sender).SelectedItem = null;
    }
}