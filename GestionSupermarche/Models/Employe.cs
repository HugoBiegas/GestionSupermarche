using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
