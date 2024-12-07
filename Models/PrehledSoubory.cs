using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledSoubory
    {
        public int IdSoubor { get; set; }
        public string NazevSouboru { get; set; }
        public string TypSouboru { get; set; }
        public string PriponaSouboru { get; set; }
        public DateTime DatumNahrani { get; set; }
    }
}
