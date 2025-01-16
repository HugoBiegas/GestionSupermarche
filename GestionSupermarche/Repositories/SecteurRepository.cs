using GestionSupermarche.Models;
using SQLite;

namespace GestionSupermarche.Repositories
{
    public class SecteurRepository
    {
        private SQLiteAsyncConnection _connection;

        public SecteurRepository(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Secteur>> ObtenirTousLesSecteurs()
        {
            return await _connection.Table<Secteur>().ToListAsync();
        }

        public async Task<Secteur> ObtenirSecteurParId(int id)
        {
            return await _connection.Table<Secteur>()
                .Where(s => s.IdSecteur == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AjouterSecteur(Secteur secteur)
        {
            return await _connection.InsertAsync(secteur);
        }

        public async Task<int> ModifierSecteur(Secteur secteur)
        {
            return await _connection.UpdateAsync(secteur);
        }

        public async Task<int> SupprimerSecteur(Secteur secteur)
        {
            return await _connection.DeleteAsync(secteur);
        }

        public async Task<int> ObtenirNombreRayonsParSecteur(int idSecteur)
        {
            return await _connection.Table<Rayon>()
                .Where(r => r.IdSecteur == idSecteur)
                .CountAsync();
        }
    }
}
