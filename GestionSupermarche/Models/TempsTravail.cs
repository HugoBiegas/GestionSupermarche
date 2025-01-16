using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public double Temps { get; set; }
    }
}
