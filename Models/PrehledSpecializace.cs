using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledSpecializace
    {
        public int id_specializace { get; set; }
        public string nazev { get; set; }
        public int stupen_narocnosti { get; set; }
        public string popis { get; set; }
    }
}
