using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Zbran
    {
        public int IdZbran { get; set; }
        public string Nazev { get; set; }
        public string Typ { get; set; }
        public string Kalibr { get; set; }
        public DateTime DatumPorizeni { get; set; }
    }
}
