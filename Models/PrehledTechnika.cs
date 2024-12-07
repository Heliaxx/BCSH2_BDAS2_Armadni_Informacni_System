using System;

namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledTechnika
    {
        public int? IdTechnika { get; set; }
        public string Typ { get; set; }
        public string RegistracniCislo { get; set; }
        public DateTime? DatumPorizeni { get; set; }
        public string Vyrobce { get; set; }
        public string Puvod { get; set; }
        public string MestoVyroby { get; set; }
        public string CisloVyrobnichPlanu { get; set; }
        public string ZemeImportu { get; set; }
        public int IdUtvar { get; set; }
        public string PatriPodUtvar { get; set; }
    }
}
