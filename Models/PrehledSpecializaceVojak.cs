using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledSpecializaceVojak
    {
        public int IdSpecializace { get; set; }
        public string Nazev { get; set; }
        public int StupenNarocnosti { get; set; }
        public string Popis { get; set; }
        public int IdVojak { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
    }
}
