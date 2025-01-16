using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSupermarche.DTO
{
    public class TempsTravailDetailDto
    {
        public int IdTempsTravail { get; set; }
        public string NomEmploye { get; set; }
        public string NomRayon { get; set; }
        public DateTime Date { get; set; }
        public double Temps { get; set; }
    }
}
