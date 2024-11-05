using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Vojaci
    {
        public int IdVojak { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public DateTime DatumNastupu { get; set; }
        public DateTime DatumPropusteni { get; set; }
        public int IdHodnost { get; set; }
        public int IdJednotka { get; set; }
        public int IdZbran { get; set; }
    }
}
