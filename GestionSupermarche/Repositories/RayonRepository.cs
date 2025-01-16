using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using GestionSupermarche.Models;

namespace GestionSupermarche.Repositories
{
    public class RayonRepository
    {
        private SQLiteAsyncConnection _connection;

        public RayonRepository(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Rayon>> ObtenirTousLesRayons()
        {
            return await _connection.Table<Rayon>().ToListAsync();
        }

        public async Task<List<Rayon>> ObtenirRayonsParSecteur(int idSecteur)
        {
            return await _connection.Table<Rayon>()
                .Where(r => r.IdSecteur == idSecteur)
                .ToListAsync();
        }

        public async Task<Rayon> ObtenirRayonParId(int id)
        {
            return await _connection.Table<Rayon>()
                .Where(r => r.IdRayon == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AjouterRayon(Rayon rayon)
        {
            return await _connection.InsertAsync(rayon);
        }

        public async Task<int> ModifierRayon(Rayon rayon)
        {
            return await _connection.UpdateAsync(rayon);
        }

        public async Task<int> SupprimerRayon(Rayon rayon)
        {
            return await _connection.DeleteAsync(rayon);
        }

        public async Task<double> CalculerTotalHeuresParRayon(int idRayon)
        {
            var temps = await _connection.Table<TempsTravail>()
                .Where(t => t.IdRayon == idRayon)
                .ToListAsync();
            return temps.Sum(t => t.Temps);
        }
    }
}
