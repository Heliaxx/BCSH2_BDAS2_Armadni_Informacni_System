using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Skoleni
    {
        public int IdSkoleni { get; set; }
        public string Nazev { get; set; }
        public DateTime DatumKonani { get; set; }
        public string MistoKonani { get; set; }
        public int PocetUcastniku { get; set; }
    }
}
