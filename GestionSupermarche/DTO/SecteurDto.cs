using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionSupermarche.DTO
{
    public class SecteurDto
    {
        public int IdSecteur { get; set; }
        public string Nom { get; set; }
        public int NombreRayons { get; set; }
        public bool PeutSupprimer { get; set; }
    }

}
