using SQLite;

namespace GestionSupermarche.Models
{
    [Table("Secteur")]
    public class Secteur
    {
        [PrimaryKey, AutoIncrement, Column("IdSecteur")]
        public int IdSecteur { get; set; }

        [Column("Nom")]
        public string Nom { get; set; }
    }
}
