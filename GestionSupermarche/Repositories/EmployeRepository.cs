using GestionSupermarche.Models;
using SQLite;

namespace GestionSupermarche.Repositories
{
    public class EmployeRepository
    {
        private SQLiteAsyncConnection _connection;

        public EmployeRepository(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Employe>> ObtenirTousLesEmployes()
        {
            return await _connection.Table<Employe>().ToListAsync();
        }

        public async Task<Employe> ObtenirEmployeParId(int id)
        {
            return await _connection.Table<Employe>()
                .Where(e => e.IdEmploye == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> AjouterEmploye(Employe employe)
        {
            return await _connection.InsertAsync(employe);
        }

        public async Task<int> ModifierEmploye(Employe employe)
        {
            return await _connection.UpdateAsync(employe);
        }

        public async Task<int> SupprimerEmploye(Employe employe)
        {
            return await _connection.DeleteAsync(employe);
        }
    }
}
