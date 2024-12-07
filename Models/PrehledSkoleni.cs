using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledSkoleni
    {
        public int? IdSkoleni { get; set; }
        public string Nazev { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public string Misto { get; set; }
        public int PocetUcastniku { get; set; }
    }
}
