using SQLite;

namespace GestionSupermarche.Models
{
    [Table("Employe")]
    public class Employe
    {
        [PrimaryKey, AutoIncrement, Column("IdEmploye")]
        public int IdEmploye { get; set; }

        [Column("Nom")]
        public string Nom { get; set; }
    }
}
