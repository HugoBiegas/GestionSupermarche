using GestionSupermarche.Models;
using SQLite;

namespace GestionSupermarche.Repositories
{
    public class TempsTravailRepository
    {
        private SQLiteAsyncConnection _connection;

        public TempsTravailRepository(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<TempsTravail>> ObtenirTousLesTemps()
        {
            return await _connection.Table<TempsTravail>().ToListAsync();
        }

        public async Task<double> GetHeuresAujourdhui(int idEmploye)
        {
            var tempsAujourdhui = await ObtenirTempsParEmploye(idEmploye);
            return tempsAujourdhui
                .Where(t => t.Date.Date == DateTime.Today)
                .Sum(t => t.Temps);
        }

        public async Task<List<TempsTravail>> ObtenirTempsParEmploye(int idEmploye)
        {
            return await _connection.Table<TempsTravail>()
                .Where(t => t.IdEmploye == idEmploye)
                .ToListAsync();
        }
        public async Task<TempsTravail> GetTempsTravailById(int id)
        {
            return await _connection
                .Table<TempsTravail>()
                .Where(t => t.IdTempsTravail == id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> VerifierDisponibiliteEmploye(int idEmploye, int idRayon, DateTime date)
        {
            var dateRecherchee = date.Date;

            var tempsTravail = await _connection.Table<TempsTravail>()
                .Where(t => t.IdEmploye == idEmploye && t.IdRayon == idRayon)
                .ToListAsync();

            var travailExistant = tempsTravail.Any(t => t.Date.Date == dateRecherchee);

            return !travailExistant;
        }

        public async Task<int> AjouterTempsTravail(TempsTravail tempsTravail)
        {
            if (!await VerifierDisponibiliteEmploye(tempsTravail.IdEmploye,
                tempsTravail.IdRayon, tempsTravail.Date))
            {
                throw new Exception("L'employé travaille déjà dans ce rayon à cette date");
            }
            return await _connection.InsertAsync(tempsTravail);
        }

        public async Task<double> CalculerTotalHeuresEmploye(int idEmploye)
        {
            var temps = await _connection.Table<TempsTravail>()
                .Where(t => t.IdEmploye == idEmploye)
                .ToListAsync();
            return temps.Sum(t => t.Temps);
        }

        public async Task<double> CalculerTotalHeuresParMois(int annee, int mois)
        {
            var temps = await _connection.Table<TempsTravail>()
                .Where(t => t.Date.Year == annee && t.Date.Month == mois)
                .ToListAsync();
            return temps.Sum(t => t.Temps);
        }

        public async Task<double> CalculerTotalHeuresGeneral()
        {
            var temps = await _connection.Table<TempsTravail>().ToListAsync();
            return temps.Sum(t => t.Temps);
        }

        public async Task<int> ModifierTempsTravail(TempsTravail tempsTravail)
        {
            return await _connection.UpdateAsync(tempsTravail);
        }

        public async Task<int> SupprimerTempsTravail(TempsTravail tempsTravail)
        {
            return await _connection.DeleteAsync(tempsTravail);
        }
    }
}
