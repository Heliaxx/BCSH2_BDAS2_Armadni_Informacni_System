using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Entities
{
    public class Dovolenky
    {
        public int IdDovolenka { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public string Duvod { get; set; }
    }
}
