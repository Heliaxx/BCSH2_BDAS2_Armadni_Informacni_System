using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.ViewModels
{
    public class ProfilUzivatele
    {
        public int IdVojak { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public DateTime DatumNastupu { get; set; }
        public DateTime? DatumPropusteni { get; set; }
        public string Email { get; set; }
        public string Jednotka { get; set; }
        public string Hodnost { get; set; }
        public string Role { get; set; }
        public string Specializace { get; set; }
    }
}
