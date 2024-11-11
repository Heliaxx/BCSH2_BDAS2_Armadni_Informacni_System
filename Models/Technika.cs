using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Technika
    {
        public int IdTechnika { get; set; }
        public string Typ { get; set; }
        public string RegistracniCislo { get; set; }
        public DateTime DatumPorizeni { get; set; }
        public string Vyrobce { get; set; }
        public char Puvod { get; set; }
    }
}
