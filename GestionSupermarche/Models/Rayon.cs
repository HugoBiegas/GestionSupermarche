using SQLite;

namespace GestionSupermarche.Models
{
    [Table("Rayon")]
    public class Rayon
    {
        [PrimaryKey, AutoIncrement, Column("IdRayon")]
        public int IdRayon { get; set; }

        [Column("Nom")]
        public string Nom { get; set; }

        [Column("IdSecteur")]
        public int IdSecteur { get; set; }
    }
}
