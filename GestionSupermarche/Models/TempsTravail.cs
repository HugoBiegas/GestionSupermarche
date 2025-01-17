using SQLite;

namespace GestionSupermarche.Models
{
    [Table("TempsTravail")]
    public class TempsTravail
    {
        [PrimaryKey, AutoIncrement, Column("IdTempsTravail")]
        public int IdTempsTravail { get; set; }

        [Column("IdEmploye")]
        public int IdEmploye { get; set; }

        [Column("IdRayon")]
        public int IdRayon { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }

        [Column("Temps")]
        public int Temps { get; set; }
    }
}
