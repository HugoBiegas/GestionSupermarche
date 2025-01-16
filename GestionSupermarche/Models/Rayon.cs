using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
