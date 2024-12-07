namespace BCSH2_BDAS2_Armadni_Informacni_System.Models
{
    public class Nadrizeni
    {
        public int IdVojak { get; set; } 
        public int LevelVojaka { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Hodnost { get; set; }
        public int? IdPrimyNadrizeny { get; set; }
    }
}
