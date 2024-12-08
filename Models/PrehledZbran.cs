using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledZbran
    {
        public int IdZbran { get; set; }
        public string NazevZbrane { get; set; }
        public DateTime? DatumPorizeni { get; set; }
        public string Typ { get; set; }
        public string Kalibr { get; set; }
        public int IdUtvar { get; set; }
        public string NazevUtvaru { get; set; }
        public int? IdVojak { get; set; }
        public string VojakJmeno { get; set; }
        public string VojakPrijmeni { get; set; }
    }
}
