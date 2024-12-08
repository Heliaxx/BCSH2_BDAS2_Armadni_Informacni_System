using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledSkoleniUcastnici
    {
        public string Skoleni { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public string Misto { get; set; }
        public int IdVojak { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public int IdSkoleni { get; set; }
    }
}
