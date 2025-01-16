using GestionSupermarche.Models;
using SQLite;


namespace GestionSupermarche.Services
{
    public class Database
    {
        private const string DB_NAME = "supermarche.db";
        private static SQLiteAsyncConnection _connection;

        public Database()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        }

        public async Task InitializeAsync()
        {
            if (_connection == null)
                return;

            await _connection.CreateTableAsync<Employe>();
            await _connection.CreateTableAsync<Secteur>();
            await _connection.CreateTableAsync<Rayon>();
            await _connection.CreateTableAsync<TempsTravail>();

            // Vérifier si des données existent déjà
            //int num = await _connection.Table<Employe>().CountAsync();
            //if (num == 0)
            //{
            //    // Ajouter des données de test
            //    var employe = new Employe { Nom = "Fortin" };
            //    await _connection.InsertAsync(employe);
            //    employe = new Employe { Nom = "Alison" };
            //    await _connection.InsertAsync(employe);
            //    employe = new Employe { Nom = "Cousinot" };
            //    await _connection.InsertAsync(employe);

            //    var secteur = new Secteur { Nom = "Alimentation" };
            //    await _connection.InsertAsync(secteur);
            //    secteur = new Secteur { Nom = "Maison" };
            //    await _connection.InsertAsync(secteur);
            //    secteur = new Secteur { Nom = "Produits frais" };
            //    await _connection.InsertAsync(secteur);

            //    var rayon = new Rayon { Nom = "Gâteaux", IdSecteur = 1 };
            //    await _connection.InsertAsync(rayon);
            //    rayon = new Rayon { Nom = "Légumes", IdSecteur = 3 };
            //    await _connection.InsertAsync(rayon);
            //    rayon = new Rayon { Nom = "Viande", IdSecteur = 3 };
            //    await _connection.InsertAsync(rayon);
            //    rayon = new Rayon { Nom = "Luminaire", IdSecteur = 2 };
            //    await _connection.InsertAsync(rayon);
            //}
        }
        public static SQLiteAsyncConnection GetConnection()
        {
            return _connection;
        }
    }
}
