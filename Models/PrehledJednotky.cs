namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class PrehledJednotky
    {
        public int? IdJednotka { get; set; }
        public string Nazev { get; set; }
        public string Typ { get; set; }
        public int Velikost { get; set; }
        public int IdUtvar { get; set; }
        public string PatriPodUtvar { get; set; }
    }
}
